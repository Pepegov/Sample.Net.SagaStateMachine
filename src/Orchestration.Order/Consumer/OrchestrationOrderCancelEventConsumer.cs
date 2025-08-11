using Orchestration.Contracts.Orchestration;
using MassTransit;
using Service.Interface;

namespace Orchestration.Order.Consumer;

public class OrchestrationOrderCancelEventConsumer(IOrderService orderService) : IConsumer<OrchestrationOrderCancelEvent>
{
    public Task Consume(ConsumeContext<OrchestrationOrderCancelEvent> context)
        => orderService.DeleteAsync(context.Message.OrderId, context.CancellationToken);
}