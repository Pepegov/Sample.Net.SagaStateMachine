using System.Net;
using Orchestration.Contracts;
using Orchestration.Contracts.Orchestration;
using MassTransit;
using Service.Interface;

namespace Orchestration.Inventory.Consumer;

public class OrchestrationInventoryGoodsBookedInWarehouseEventConsumer(IInventoryService inventoryService) : IConsumer<OrchestrationInventoryGoodsBookedInWarehouseEvent>
{
    public async Task Consume(ConsumeContext<OrchestrationInventoryGoodsBookedInWarehouseEvent> context)
    {
        try
        {
            await inventoryService.BookGoodsAsync(context.Message.GoodBooks, context.CancellationToken);
        }
        catch (ArgumentOutOfRangeException e)
        {
            await context.Publish(new OrchestrationInventoryGoodsBookedInWarehouseEventFailed(context.Message.OrderId,
                new ProblemDetails()
                {
                    Details = e.Message,
                    Instance = nameof(OrchestrationInventoryGoodsBookedInWarehouseEventConsumer),
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = HttpStatusCode.InternalServerError.ToString(),
                    Type = "BookError"
                }));
            return;
        }

        await context.Publish(new OrchestrationInventoryGoodsBookedInWarehouseEventCompleted(context.Message.OrderId));
    }
}