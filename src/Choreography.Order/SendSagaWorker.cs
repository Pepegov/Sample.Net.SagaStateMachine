using System.Net;
using Choreography.Contracts;
using Choreography.Contracts.Order;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Model;

namespace Choreography.Order;

public class SendSagaWorker(ILogger<SendSagaWorker> logger, IRequestClient<OrderCreateCommand> orderCreateCommandMqClient) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(1000);
        Console.WriteLine("Press enter to continue");
        Console.ReadKey();
        
        //Arrange
        cancellationToken = default;
        var cartItems = new List<GoodViewModel>() { Constans.Good };
        var address = "7811 NE Pleasant Valley RdLiberty, Missouri(MO), 64068";
        
        await InitializeSagaAsync(Constans.UserId, cartItems, address, cancellationToken);
    }

    private async Task InitializeSagaAsync(Guid userId, IEnumerable<GoodViewModel> cartItems, string address,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var orderCreateCommand = new OrderCreateCommand(userId, cartItems, address);
            var orderCreateResponse =
                await orderCreateCommandMqClient.GetResponse<MqResult<Guid>>(orderCreateCommand, cancellationToken);

            if (orderCreateResponse.Message.ProblemDetails is not null)
            {
                var errorMessage = $"{nameof(OrderCreateCommand)} faulted " +
                                   $"| problem: {orderCreateResponse.Message.ProblemDetails}";
                logger.LogError(errorMessage);
            }
        }
        catch (MassTransitException exception)
        {
            var errorMessage = $"Unable to send {nameof(OrderCreateCommand)} request to queue " +
                               $"| exception: {exception.Message}";
            logger.LogCritical(errorMessage);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}