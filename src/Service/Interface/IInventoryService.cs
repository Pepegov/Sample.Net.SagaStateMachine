using DAL;
using DAL.Model;

namespace Service.Interface;

public interface IInventoryService
{
    Task<Dictionary<Guid, bool>> CheckAvailabilityAsync(IEnumerable<Guid> goodIds, CancellationToken cancellationToken = default);
    Task BookGoodsAsync(Dictionary<Guid, int> goodDictionary, CancellationToken cancellationToken = default);
    Task AddGoodsCountAsync(Dictionary<Guid, int> goodDictionary, CancellationToken cancellationToken = default);
    Task InsertAsync(IEnumerable<Good> goods, CancellationToken cancellationToken = default);
}