using System.Net;
using Orchestration.Contracts;
using Orchestration.Contracts.Orchestration;
using MassTransit;
using Service.Interface;
using Service.Model;

namespace Orchestration.Order.Consumer;

public class OrchestrationOrderCreateEventConsumer(IOrderService orderService) : IConsumer<OrchestrationOrderCreateEvent>
{
    public async Task Consume(ConsumeContext<OrchestrationOrderCreateEvent> context)
    {
        var orderCreationModel = new OrderCreationModel
        {
            CartItems = context.Message.Goods.Select(x => x.Name)
                .ToList(),
            Amount = context.Message.Goods.Sum(x => x.Price * x.Count),
            UserId = context.Message.UserId,
            DeliveryAddress = context.Message.DeliveryAddress,
        };
        
        try
        {
            await orderService.InsertAsync(orderCreationModel, context.Message.OrderId, context.CancellationToken);
        }
        catch (Exception e)
        {
            await context.Publish(new OrchestrationOrderCreateEventFailed(context.Message.OrderId,
                new ProblemDetails()
                {
                    Details = e.Message,
                    Instance = nameof(OrchestrationOrderCreateEventConsumer),
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = HttpStatusCode.InternalServerError.ToString(),
                    Type = "DatabaseError"
                }));
            return;
        }

        await context.Publish(new OrchestrationOrderCreateEventCompleted(context.Message.OrderId));
    }
}