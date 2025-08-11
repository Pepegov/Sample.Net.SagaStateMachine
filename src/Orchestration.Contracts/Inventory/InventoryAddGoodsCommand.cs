using Service.Model;

namespace Orchestration.Contracts.Inventory;

/// <summary>
/// Add good count in inventory
/// </summary>
/// <param name="goods"></param>
/// <returns>MqResult of bool</returns>
public class InventoryAddGoodsCommand(IEnumerable<GoodViewModel> goods);