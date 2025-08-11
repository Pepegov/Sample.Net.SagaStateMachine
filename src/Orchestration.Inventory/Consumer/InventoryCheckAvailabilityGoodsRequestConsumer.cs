using Orchestration.Contracts;
using MassTransit;
using Orchestration.Contracts.Inventory;
using Service.Interface;

namespace Orchestration.Inventory.Consumer;

public class InventoryCheckAvailabilityGoodsRequestConsumer(IInventoryService inventoryService) : IConsumer<InventoryCheckAvailabilityGoodsRequest>
{
    public async Task Consume(ConsumeContext<InventoryCheckAvailabilityGoodsRequest> context)
    {
        var checkAvailabilityResult = await inventoryService.CheckAvailabilityAsync(context.Message.GoodIds, context.CancellationToken);
        await context.RespondAsync(new MqResult<Dictionary<Guid, bool>>(checkAvailabilityResult));
    }
}