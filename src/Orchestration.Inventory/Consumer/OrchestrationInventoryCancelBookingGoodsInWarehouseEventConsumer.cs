using Orchestration.Contracts.Orchestration;
using MassTransit;
using Service.Interface;

namespace Orchestration.Inventory.Consumer;

public class OrchestrationInventoryCancelBookingGoodsInWarehouseEventConsumer(IInventoryService inventoryService) : IConsumer<OrchestrationInventoryCancelBookingGoodsInWarehouseEvent>
{
    public Task Consume(ConsumeContext<OrchestrationInventoryCancelBookingGoodsInWarehouseEvent> context)
        => inventoryService.AddGoodsCountAsync(context.Message.Goods, context.CancellationToken);
}