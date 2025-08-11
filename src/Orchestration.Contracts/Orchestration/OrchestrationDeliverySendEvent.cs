using Service.Model;

namespace Orchestration.Contracts.Orchestration;

public record OrchestrationDeliverySendEvent(Guid OrderId, IEnumerable<GoodViewModel> Goods, Guid UserId, string Address);