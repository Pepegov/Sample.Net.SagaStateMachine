namespace Orchestration.Contracts.Orchestration;

public record OrchestrationInventoryCancelBookingGoodsInWarehouseEvent(Guid OrderId, Dictionary<Guid, int> Goods);