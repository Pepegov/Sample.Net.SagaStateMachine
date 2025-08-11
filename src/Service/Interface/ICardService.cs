using DAL.Model;

namespace Service.Interface;

public interface ICardService
{
    Task<Card?> GetFirstCardByUserId(Guid userId, CancellationToken cancellationToken = default);
}