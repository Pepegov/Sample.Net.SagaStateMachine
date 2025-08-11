using DAL;
using DAL.Enums;
using DAL.Model;
using Microsoft.EntityFrameworkCore;
using Service.Interface;
using Service.Model;

namespace Service;

public class OrderService(ApplicationDbContext dbContext) : IOrderService
{
    public async Task InsertAsync(OrderCreationModel creationModel, Guid? orderId = null, CancellationToken cancellationToken = default)
    {
        var order = new Order
        {
            Amount = creationModel.Amount,
            CartItems = creationModel.CartItems,
            CreationAtUtc = DateTime.UtcNow,
            Status = OrderStatus.Preparing,
            Id = orderId ?? Guid.NewGuid(),
            UserId = creationModel.UserId,
            DeliveryAddress = creationModel.DeliveryAddress
        };

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var firstOrDefault = await dbContext.Orders.FirstOrDefaultAsync(f => f.Id == orderId, cancellationToken: cancellationToken);
        if (firstOrDefault is null)
        {
            return;
        }
        
        dbContext.Remove(firstOrDefault);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}