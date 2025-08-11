namespace Service.Interface;

public interface IDeliveryService
{
    Task SendPackageAsync(Guid orderId, ICollection<Guid> goodIds, Guid UserId, string address, CancellationToken cancellationToken = default);
}