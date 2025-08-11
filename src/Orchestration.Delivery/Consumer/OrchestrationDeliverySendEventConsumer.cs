using System.Net;
using MassTransit;
using Orchestration.Contracts;
using Orchestration.Contracts.Orchestration;
using Service.Interface;

namespace Orchestration.Delivery.Consumer;

public class OrchestrationDeliverySendEventConsumer(IDeliveryService deliveryService) : IConsumer<OrchestrationDeliverySendEvent>
{
    public async Task Consume(ConsumeContext<OrchestrationDeliverySendEvent> context)
    {
        try
        {
            await deliveryService.SendPackageAsync(context.Message.OrderId, context.Message.Goods.Select(x => x.Id).ToList(),
                context.Message.UserId, context.Message.Address, context.CancellationToken);
        }
        catch (Exception e)
        {
            await context.Publish(new OrchestrationDeliverySendEventFailed(context.Message.OrderId,
                new ProblemDetails()
                {
                    Details = e.Message,
                    Instance = nameof(OrchestrationDeliverySendEventConsumer),
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = HttpStatusCode.InternalServerError.ToString(),
                    Type = "DeliveryError"
                }));
            return;
        }

        await context.Publish(new OrchestrationDeliverySendEventCompleted(context.Message.OrderId));
    }
}