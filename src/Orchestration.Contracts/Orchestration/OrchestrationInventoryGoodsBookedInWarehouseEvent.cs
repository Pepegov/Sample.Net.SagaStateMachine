namespace Orchestration.Contracts.Orchestration;

public record OrchestrationInventoryGoodsBookedInWarehouseEvent(Guid OrderId, Dictionary<Guid, int> GoodBooks);