using DAL;
using DAL.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Model;

namespace Orchestration.Inventory;

public class SeedingDataWorker(ApplicationDbContext applicationDbContext, ILogger<SeedingDataWorker> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var good = new Good()
        {
            Id = Constans.Good.Id,
            Name = Constans.Good.Name,
            Count = Constans.Good.Count
        };
        applicationDbContext.Goods.Add(good);
        logger.LogInformation($"[{nameof(SeedingDataWorker)}] Successfully seed good data in Inventory service");
        return applicationDbContext.SaveChangesAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}