using System.Net;
using Choreography.Contracts;
using Choreography.Contracts.Order;
using MassTransit;
using Microsoft.Extensions.Logging;
using Service.Interface;
using Service.Model;

namespace Choreography.Order.Consumer;

public class OrderCreateCommandConsumer(ILogger<OrderCreateCommandConsumer> logger, ICardService cardService, IOrderService orderService) : IConsumer<OrderCreateCommand>
{
    public async Task Consume(ConsumeContext<OrderCreateCommand> context)
    {
        var card = await cardService.GetFirstCardByUserId(context.Message.UserId, context.CancellationToken);
        if (card is null)
        {
            var errorMessage = $"The user {context.Message.UserId} did not save any card";
            logger.LogError($"[{nameof(OrderCreateCommandConsumer)}]. Message: {errorMessage}");
            var problemDetails = new ProblemDetails()
            {
                Details = errorMessage,
                Instance = nameof(OrderCreateCommandConsumer),
                Status = (int)HttpStatusCode.InternalServerError,
                Title = HttpStatusCode.InternalServerError.ToString(),
                Type = "CardError"
            };
            await context.RespondAsync(new MqResult<Guid>(problemDetails));
            return;
        }

        var orderId = Guid.NewGuid();

        var orderCreationModel = new OrderCreationModel
        {
            UserId = context.Message.UserId,
            DeliveryAddress = context.Message.Address,
            CartItems = context.Message.CartItems.Select(x => x.Name).ToList(),
            Amount = context.Message.CartItems.Sum(x => x.Price * x.Count)
        };

        try
        {
            await orderService.InsertAsync(orderCreationModel, orderId, context.CancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError($"[{nameof(OrderCreateCommandConsumer)}]. Message: {e.Message}");
            var problemDetails = new ProblemDetails()
            {
                Details = e.Message,
                Instance = nameof(OrderCreateCommandConsumer),
                Status = (int)HttpStatusCode.InternalServerError,
                Title = HttpStatusCode.InternalServerError.ToString(),
                Type = "OrderError"
            };
            await context.Publish(new OrderCreateEventFailed(orderId, problemDetails));
            await context.RespondAsync(new MqResult<Guid>(problemDetails));
            return;
        }
        
        await context.Publish(new OrderCreateEventCompleted(orderId, context.Message.UserId, context.Message.CartItems, context.Message.Address));
        await context.RespondAsync(new MqResult<Guid>(orderId));
        logger.LogCritical($"[{nameof(OrderCreateCommandConsumer)}]. Message: Successfully create order {orderId}");
    }
}