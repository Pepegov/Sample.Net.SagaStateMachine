using System.Net;
using Choreography.Contracts;
using Choreography.Contracts.Delivery;
using Choreography.Contracts.Inventory;
using MassTransit;
using Microsoft.Extensions.Logging;
using Service.Interface;

namespace Choreography.Delivery.Consumer;

public class InventoryGoodsBookedInWarehouseEventCompletedConsumer(ILogger<InventoryGoodsBookedInWarehouseEventCompletedConsumer> logger, IDeliveryService deliveryService) : IConsumer<InventoryGoodsBookedInWarehouseEventCompleted>
{
    public async Task Consume(ConsumeContext<InventoryGoodsBookedInWarehouseEventCompleted> context)
    {
        try
        {
            await deliveryService.SendPackageAsync(context.Message.OrderId, context.Message.CartItems.Select(x => x.Id).ToList(),
                context.Message.UserId, context.Message.Address, context.CancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError($"[{nameof(InventoryGoodsBookedInWarehouseEventCompletedConsumer)}]. Message: {e.Message}");
            await context.Publish(new DeliverySendEventFailed(context.Message.OrderId, context.Message.CartItems,
                new ProblemDetails()
                {
                    Details = e.Message,
                    Instance = nameof(InventoryGoodsBookedInWarehouseEventCompletedConsumer),
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = HttpStatusCode.InternalServerError.ToString(),
                    Type = "DeliveryError"
                }));
            return;
        }

        await context.Publish(new DeliverySendEventCompleted(context.Message.OrderId), context.CancellationToken);
        logger.LogCritical($"[{nameof(InventoryGoodsBookedInWarehouseEventCompletedConsumer)}]. Message: Successfully send delivery by orderId {context.Message.OrderId}");
    }
}