using System.Net;
using System.Runtime.Serialization;
using Orchestration.Contracts;
using Orchestration.Contracts.Orchestration;
using DAL;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orchestration.Contracts.Inventory;
using Orchestration.Contracts.Order;
using Service.Model;

namespace Orchestration.Saga;

public class SendSagaWorker : IHostedService
{
    private readonly IRequestClient<OrderGetFirstCardRequest> _cardIsBelongsUserClient;
    private readonly IRequestClient<InventoryCheckAvailabilityGoodsRequest> _checkAvailabilityGoodsClient;
    private readonly IRequestClient<OrchestrationInitializeSagaEvent> _initializeSagaClient;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<SendSagaWorker> _logger;

    public SendSagaWorker(IRequestClient<OrderGetFirstCardRequest> cardIsBelongsUserClient, ILogger<SendSagaWorker> logger, IRequestClient<OrchestrationInitializeSagaEvent> initializeSagaClient, IRequestClient<InventoryCheckAvailabilityGoodsRequest> checkAvailabilityGoodsClient, IPublishEndpoint publishEndpoint)
    {
        _cardIsBelongsUserClient = cardIsBelongsUserClient;
        _logger = logger;
        _initializeSagaClient = initializeSagaClient;
        _checkAvailabilityGoodsClient = checkAvailabilityGoodsClient;
        _publishEndpoint = publishEndpoint;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(1000);
        Console.WriteLine("Press enter to continue");
        Console.ReadKey();
        
        //Arrange
        cancellationToken = default;
        var orderId = Guid.NewGuid();
        var cartItems = new List<GoodViewModel>() { Constans.Good };
        var address = "7811 NE Pleasant Valley RdLiberty, Missouri(MO), 64068";

        //Get a saved user card
        var cardIsBelongsUserResponse = await _cardIsBelongsUserClient.GetResponse<MqResult<CardDto>>(new OrderGetFirstCardRequest(Constans.UserId), cancellationToken);
        if (cardIsBelongsUserResponse.Message.ProblemDetails is not null)
        {
            _logger.LogError($"The user did not save the card. ProblemDetails: {cardIsBelongsUserResponse.Message.ProblemDetails}");
            return;
        }

        //Checking the availability of goods
        var goodIds = cartItems.Select(s => s.Id);
        var checkAvailabilityGoodsResponse = await _checkAvailabilityGoodsClient.GetResponse<MqResult<Dictionary<Guid, bool>>>(new InventoryCheckAvailabilityGoodsRequest(goodIds), cancellationToken);
        if (checkAvailabilityGoodsResponse.Message.ProblemDetails is not null)
        {
            _logger.LogError($"Some goods are not available. ProblemDetails: {checkAvailabilityGoodsResponse.Message.ProblemDetails}");
            return;
        }
        ValidateGoodsAvailability(cartItems, checkAvailabilityGoodsResponse.Message.Model);

        //Initialize Saga
        await InitializeSagaAsync(orderId, cartItems, Constans.UserId, address, cancellationToken);
        _logger.LogCritical("AMAZING! Orchestration saga successfully completed!");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task InitializeSagaAsync(Guid orderId, IEnumerable<GoodViewModel> goods, Guid userId, string address, CancellationToken cancellationToken = default)
    {
        try
        {
            var sagaResponse = await _initializeSagaClient.GetResponse<SagaResponse>(
                new OrchestrationInitializeSagaEvent(orderId, goods, userId, address), cancellationToken);
            if (sagaResponse.Message.ProblemDetails is not null)
            {
                var errorMessage = $"{nameof(OrchestrationInitializeSagaEvent)} faulted " +
                                   $"| problem: {sagaResponse.Message.ProblemDetails}";
                _logger.LogError(errorMessage);
            }
        }
        catch (MassTransitException exception)
        {
            var errorMessage = $"Unable to send {nameof(OrchestrationInitializeSagaEvent)} request to queue " +
                               $"| exception: {exception.Message}";
            _logger.LogCritical(errorMessage);
            if (exception is RequestTimeoutException timeoutException)
            {
                //Fallback
                var request = new OrchestrationDeliverySendEventFailed(orderId, timeoutException.ToProblemDetails(HttpStatusCode.InternalServerError, nameof(OrchestrationInitializeSagaEvent)));
                await _publishEndpoint.Publish(request, cancellationToken);
            }
        }
    }


    private void ValidateGoodsAvailability(
        List<GoodViewModel> goods,
        Dictionary<Guid, bool> availabilityDictionary)
    {
        //Collect all the good IDs for a quick search
        var goodsIds = new HashSet<Guid>(goods.Select(g => g.Id));

        // Check that all products are in the availability dictionary.
        if (goodsIds.Except(availabilityDictionary.Keys).Any())
        {
            var missingIds = goodsIds.Except(availabilityDictionary.Keys);
            throw new InvalidOperationException(
                $"Some good are missing from the availability list: {string.Join(", ", missingIds)}");
        }

        // Check that all good are available (true)
        var unavailableGoods = goods
            .Where(g => !availabilityDictionary[g.Id])
            .Select(g => g.Name)
            .ToList();

        if (unavailableGoods.Any())
        {
            throw new InvalidOperationException(
                $"Some goods are not available: {string.Join(", ", unavailableGoods)}");
        }
    }
}

[Serializable]
public class MessageQueueException : Exception
{
    public MessageQueueException() { }
    public MessageQueueException(string message) : base(message) { }

    public MessageQueueException(string message, Exception innerException) : base(message, innerException) { }

    protected MessageQueueException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}