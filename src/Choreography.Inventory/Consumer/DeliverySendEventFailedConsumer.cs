using Choreography.Contracts.Delivery;
using MassTransit;
using Microsoft.Extensions.Logging;
using Service.Interface;

namespace Choreography.Inventory.Consumer;

public class DeliverySendEventFailedConsumer(ILogger<DeliverySendEventFailedConsumer> logger, IInventoryService inventoryService) : IConsumer<DeliverySendEventFailed>
{
    public async Task Consume(ConsumeContext<DeliverySendEventFailed> context)
    {
        await inventoryService.AddGoodsCountAsync(context.Message.CartItems.ToDictionary(x => x.Id, i => i.Count), context.CancellationToken);
        logger.LogInformation($"[{nameof(DeliverySendEventFailedConsumer)}] Message: Cancellation of the reservation of goods on order by id {context.Message.OrderId}");
    }
}