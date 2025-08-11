namespace Choreography.Contracts.Inventory;

public record InventoryGoodsBookedInWarehouseEventFailed(Guid OrderId, ProblemDetails ProblemDetails);