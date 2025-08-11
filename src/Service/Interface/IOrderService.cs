using Service.Model;

namespace Service.Interface;

public interface IOrderService
{
    Task InsertAsync(OrderCreationModel creationModel, Guid? orderId = null, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid orderId, CancellationToken cancellationToken = default);
}