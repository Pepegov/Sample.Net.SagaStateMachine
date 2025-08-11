using Service.Model;

namespace Choreography.Contracts.Order;

public record OrderCreateEventCompleted(Guid OrderId, Guid UserId, IEnumerable<GoodViewModel> CartItems, string Address);