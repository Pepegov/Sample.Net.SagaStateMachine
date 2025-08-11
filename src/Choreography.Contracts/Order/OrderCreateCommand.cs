using Service.Model;

namespace Choreography.Contracts.Order;

public record OrderCreateCommand(Guid UserId, IEnumerable<GoodViewModel> CartItems, string Address);
