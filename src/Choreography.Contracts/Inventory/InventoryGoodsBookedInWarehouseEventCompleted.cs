using Service.Model;

namespace Choreography.Contracts.Inventory;

public record InventoryGoodsBookedInWarehouseEventCompleted(Guid OrderId, Guid UserId, IEnumerable<GoodViewModel> CartItems, string Address);