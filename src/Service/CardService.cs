using DAL;
using DAL.Model;
using Microsoft.EntityFrameworkCore;
using Service.Interface;

namespace Service;

public class CardService(ApplicationDbContext dbContext) : ICardService
{
    public Task<Card?> GetFirstCardByUserId(Guid userId, CancellationToken cancellationToken = default)
        => dbContext.Cards.FirstOrDefaultAsync(f => f.UserId == userId, cancellationToken);
}