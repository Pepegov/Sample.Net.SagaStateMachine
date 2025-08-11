namespace Orchestration.Contracts.Inventory;

/// <summary>
/// Checks the goods for availability (the number of goods is more than 0)
/// </summary>
/// <param name="GoodIds"></param>
/// <returns>MqResult of Dictionary(Guid, bool) </returns>
public record InventoryCheckAvailabilityGoodsRequest(IEnumerable<Guid> GoodIds);