using Choreography.Contracts.Delivery;
using MassTransit;
using Microsoft.Extensions.Logging;
using Service.Interface;

namespace Choreography.Order.Consumer;

public class DeliverySendEventFailedConsumer(ILogger<DeliverySendEventFailedConsumer> logger, IOrderService orderService) : IConsumer<DeliverySendEventFailed>
{
    public async Task Consume(ConsumeContext<DeliverySendEventFailed> context)
    {
        await orderService.DeleteAsync(context.Message.OrderId, context.CancellationToken);
        logger.LogInformation($"[{nameof(DeliverySendEventFailedConsumer)}] Message: Cancellation of the reservation of goods on order by id {context.Message.OrderId}");
    }
}