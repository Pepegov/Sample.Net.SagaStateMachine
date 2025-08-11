using Choreography.Contracts.Inventory;
using MassTransit;
using Microsoft.Extensions.Logging;
using Service.Interface;

namespace Choreography.Order.Consumer;

public class InventoryGoodsBookedInWarehouseEventFailedConsumer(ILogger<InventoryGoodsBookedInWarehouseEventFailedConsumer> logger, IOrderService orderService) : IConsumer<InventoryGoodsBookedInWarehouseEventFailed>
{
    public async Task Consume(ConsumeContext<InventoryGoodsBookedInWarehouseEventFailed> context)
    {
        await orderService.DeleteAsync(context.Message.OrderId, context.CancellationToken);
        logger.LogInformation($"[{nameof(InventoryGoodsBookedInWarehouseEventFailedConsumer)}] Message: Delete order by id {context.Message.OrderId}. Event: {nameof(InventoryGoodsBookedInWarehouseEventFailed)}");
    }
}