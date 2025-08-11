using DAL;
using DAL.Model;
using Microsoft.EntityFrameworkCore;
using Service.Interface;

namespace Service;

public class InventoryService(ApplicationDbContext dbContext) : IInventoryService
{
    public async Task<Dictionary<Guid, bool>> CheckAvailabilityAsync(IEnumerable<Guid> goodIds, CancellationToken cancellationToken = default)
    {
        var goods = await dbContext.Goods
            .Where(good => goodIds.Contains(good.Id))
            .ToDictionaryAsync(
                good => good.Id, 
                good => good.Count > 0, 
                cancellationToken);

        // Creating a result based on missing items (false for them)
        return goodIds.ToDictionary(
            id => id, 
            id => goods.TryGetValue(id, out var isAvailable) && isAvailable);

    }

    public async Task BookGoodsAsync(Dictionary<Guid, int> goodDictionary, CancellationToken cancellationToken = default)
    {
        var ids = goodDictionary.Select(s => s.Key);
        var goods = await dbContext.Goods
            .Where(good => ids.Contains(good.Id))
            .ToListAsync(cancellationToken);

        foreach (var good in goods)
        {
            var count = goodDictionary[good.Id];
            if (good.Count < count)
            {
                throw new ArgumentOutOfRangeException($"{count-good.Count} units of {good.Name} good are missing");
            }

            good.Count -= count;
        }
        
        dbContext.Goods.UpdateRange(goods);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task AddGoodsCountAsync(Dictionary<Guid, int> goodDictionary, CancellationToken cancellationToken = default)
    {
        var ids = goodDictionary.Select(s => s.Key);
        var goods = await dbContext.Goods
            .Where(good => ids.Contains(good.Id))
            .ToListAsync(cancellationToken);

        foreach (var good in goods)
        {
            good.Count += goodDictionary[good.Id];
        }
        
        dbContext.Goods.UpdateRange(goods);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task InsertAsync(IEnumerable<Good> goods, CancellationToken cancellationToken = default)
        => dbContext.Goods.AddRangeAsync(goods, cancellationToken);
}