using DAL.Model;
using Service.Model;

namespace Choreography.Contracts.Delivery;

public record DeliverySendEventFailed(Guid OrderId, IEnumerable<GoodViewModel> CartItems, ProblemDetails ProblemDetails);