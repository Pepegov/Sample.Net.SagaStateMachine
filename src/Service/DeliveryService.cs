using DAL;
using DAL.Model;
using Service.Interface;

namespace Service;

public class DeliveryService(ApplicationDbContext applicationDbContext) : IDeliveryService
{
    public async Task SendPackageAsync(Guid orderId, ICollection<Guid> goodIds, Guid userId,  string address, CancellationToken cancellationToken = default)
    {
        if (goodIds.Count == 0)
        {
            throw new ArgumentException($"{nameof(goodIds)} cannot be equal zero items");
        }
        
        var delivery = new Delivery
        {
            Id = Guid.NewGuid(),
            Address = address,
            GoodIds = goodIds,
            OrderId = orderId,
            UserId = userId,
        };

        await applicationDbContext.Deliveries.AddAsync(delivery, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}