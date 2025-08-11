using Orchestration.Contracts;
using Orchestration.Contracts.Orchestration;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Orchestration.Saga;

public class OrderSagaStateMachine : MassTransitStateMachine<OrderSaga>
{
    private readonly ILogger<OrderSagaStateMachine> _logger;
    
    private State OrderCreated { get; set; } = null!;
    private State BookingGoodsInWarehouse { get; set; } = null!;
    private State DeliverySend { get; set; } = null!;
    private State Failed { get; set; } = null!;
    
    private Event<OrchestrationInitializeSagaEvent> OnSagaStarted { get; set; } = null!;
    private Event<OrchestrationInventoryGoodsBookedInWarehouseEventCompleted> OnGoodsBookedInWarehouseCompleted { get; set; } = null!;
    private Event<OrchestrationInventoryGoodsBookedInWarehouseEventFailed> OnGoodsBookedInWarehouseFailed { get; set; } = null!;
    private Event<OrchestrationOrderCreateEventCompleted> OnOrderCreateCompleted { get; set; } = null!;
    private Event<OrchestrationOrderCreateEventFailed> OnOrderCreateFailed { get; set; } = null!;
    private Event<OrchestrationDeliverySendEventCompleted> OnDeliverySendEventCompleted { get; set; } = null!;
    private Event<OrchestrationDeliverySendEventFailed> OnDeliverySendEventFailed { get; set; } = null!;

    public OrderSagaStateMachine(ILogger<OrderSagaStateMachine> logger)
    {
        _logger = logger;
        
        State(() => OrderCreated);
        State(() => BookingGoodsInWarehouse);
        State(() => DeliverySend);
        State(() => Failed);
        
        Event(() => OnSagaStarted, context => context.CorrelateById(x => x.Message.OrderId));
        Event(() => OnGoodsBookedInWarehouseCompleted, context => context.CorrelateById(x => x.Message.OrderId));
        Event(() => OnGoodsBookedInWarehouseFailed, context => context.CorrelateById(x => x.Message.OrderId));
        Event(() => OnOrderCreateCompleted, context => context.CorrelateById(x => x.Message.OrderId));
        Event(() => OnOrderCreateFailed, context => context.CorrelateById(x => x.Message.OrderId));
        Event(() => OnDeliverySendEventCompleted, context => context.CorrelateById(x => x.Message.OrderId));
        Event(() => OnDeliverySendEventFailed, context => context.CorrelateById(x => x.Message.OrderId));

        InstanceState(x => x.CurrentState);
        
        Initially(WhenSagaStarted());
        
        During(BookingGoodsInWarehouse,
            When(OnGoodsBookedInWarehouseCompleted)
                .Then(LogSagaState)
                .Publish(context => new OrchestrationOrderCreateEvent(context.Message.OrderId, context.Saga.Goods, context.Saga.UserId, context.Saga.DeliveryAddress))
                .TransitionTo(OrderCreated),
            When(OnGoodsBookedInWarehouseFailed)
                .Then(LogSagaState)
                .Publish(context => new OrchestrationInventoryCancelBookingGoodsInWarehouseEvent(context.CorrelationId!.Value, context.Saga.Goods.ToDictionary(id => id.Id, model => model.Count)))
                .ThenAsync(async context => await RespondFromSaga(context, new SagaResponse(context.ConversationId!.Value, context.Message.ProblemDetails)))
                .TransitionTo(Failed));
        
        During(OrderCreated,
            When(OnOrderCreateCompleted)
                .Then(LogSagaState)
                .Publish(context => new OrchestrationDeliverySendEvent(context.Message.OrderId, context.Saga.Goods, context.Saga.UserId, context.Saga.DeliveryAddress))
                .TransitionTo(DeliverySend),
            When(OnOrderCreateFailed)
                .Then(LogSagaState)
                .Publish(context => new OrchestrationInventoryCancelBookingGoodsInWarehouseEvent(context.CorrelationId!.Value, context.Saga.Goods.ToDictionary(id => id.Id, model => model.Count)))
                .ThenAsync(async context => await RespondFromSaga(context, new SagaResponse(context.CorrelationId!.Value, context.Message.ProblemDetails)))
                .TransitionTo(Failed));
        
        During(DeliverySend,
        When(OnDeliverySendEventCompleted)
                .Then(LogSagaState)
                .ThenAsync(async context => await RespondFromSaga(context, new SagaResponse(context.CorrelationId!.Value)))
                .TransitionTo(Final),
                WhenDeliverySendFailed());
        
        DuringAny(WhenDeliverySendFailed());
    }

    private EventActivityBinder<OrderSaga, OrchestrationDeliverySendEventFailed> WhenDeliverySendFailed()
        => When(OnDeliverySendEventFailed)
            .Then(LogSagaState)
            .Publish(context => new OrchestrationInventoryCancelBookingGoodsInWarehouseEvent(context.CorrelationId!.Value, context.Saga.Goods.ToDictionary(id => id.Id, model => model.Count)))
            .Publish(context => new OrchestrationOrderCancelEvent(context.CorrelationId!.Value))
            .TransitionTo(Failed);

    private EventActivityBinder<OrderSaga, OrchestrationInitializeSagaEvent> WhenSagaStarted()
    {
        return When(OnSagaStarted)
            .Then(InitializeSaga)
            .Then(LogSagaState)
            .Publish(context =>
                new OrchestrationInventoryGoodsBookedInWarehouseEvent(context.Message.OrderId, context.Message.Goods.ToDictionary(x => x.Id, model => model.Count)))
            .TransitionTo(BookingGoodsInWarehouse);
    }
    
    private void InitializeSaga(BehaviorContext<OrderSaga, OrchestrationInitializeSagaEvent> context)
    {
        context.Saga.Goods = context.Message.Goods;
        context.Saga.DeliveryAddress = context.Message.DeliveryAddress;
        context.Saga.CorrelationId = context.Message.OrderId;
        context.Saga.UserId = context.Message.UserId;
        context.Saga.RequestId = context.RequestId;
        context.Saga.ResponseAddress = context.ResponseAddress;
        context.Saga.CreatedAt = DateTime.UtcNow;
    }
    
    private void LogSagaState<TEvent>(BehaviorContext<OrderSaga, TEvent> context) where TEvent : class
    {
        _logger.LogInformation($"{nameof(OrderSaga)} | correlationId: {context.Saga.CorrelationId} | event: {context.Event.Name}");
        context.Saga.UpdateAt = DateTime.UtcNow;
    }
    
    private static async Task RespondFromSaga<TEvent>(BehaviorContext<OrderSaga, TEvent> context, SagaResponse response) where TEvent : class
    {
        var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress!);
        await endpoint.Send(response, r => r.RequestId = context.Saga.RequestId);
        context.Saga.CompletedAt = DateTime.UtcNow;
    }
}