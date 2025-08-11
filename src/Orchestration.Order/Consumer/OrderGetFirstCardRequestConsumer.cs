using System.Net;
using Orchestration.Contracts;
using MassTransit;
using Orchestration.Contracts.Order;
using Service.Interface;
using Service.Model;

namespace Orchestration.Order.Consumer;

public class OrderGetFirstCardRequestConsumer(ICardService cardService) : IConsumer<OrderGetFirstCardRequest>
{
    public async Task Consume(ConsumeContext<OrderGetFirstCardRequest> context)
    {
        var card = await cardService.GetFirstCardByUserId(context.Message.UserId, context.CancellationToken);
        MqResult<CardDto>? response;
        if (card is null)
        {
            response = new MqResult<CardDto>(new ProblemDetails()
            {
                Details = $"Card by UserId {context.Message.UserId} was not found",
                Instance = nameof(OrderGetFirstCardRequestConsumer),
                Status = (int)HttpStatusCode.NotFound,
                Title = HttpStatusCode.NotFound.ToString(),
                Type = "NotFoundError"
            });
            await context.RespondAsync(response);
        }
        
        response = new MqResult<CardDto>(new CardDto()
        {
            Id = card.Id,
            PanBlock = card.PanBlock,
            UserId = card.UserId
        });
        await context.RespondAsync(response);
    }
}