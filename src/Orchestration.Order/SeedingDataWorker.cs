using DAL;
using DAL.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Model;

namespace Orchestration.Order;

public class SeedingDataWorker(ApplicationDbContext applicationDbContext, ILogger<SeedingDataWorker> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var card = new Card()
        {
            Id = Guid.NewGuid(),
            PanBlock = "0000",
            UserId = Constans.UserId
        };
        applicationDbContext.Cards.Add(card);
        logger.LogInformation($"[{nameof(SeedingDataWorker)}] Successfully seed card data in Order service");
        return applicationDbContext.SaveChangesAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}