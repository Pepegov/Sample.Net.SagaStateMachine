namespace Orchestration.Contracts.Orchestration;

public record OrchestrationDeliverySendEventFailed(Guid OrderId, ProblemDetails ProblemDetails);