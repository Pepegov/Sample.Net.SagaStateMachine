using Service.Model;

namespace Orchestration.Contracts.Orchestration;

public record OrchestrationOrderCreateEvent(Guid OrderId, IEnumerable<GoodViewModel> Goods, Guid UserId, string DeliveryAddress);