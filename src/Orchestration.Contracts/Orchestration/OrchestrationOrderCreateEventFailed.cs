namespace Orchestration.Contracts.Orchestration;

public record OrchestrationOrderCreateEventFailed(Guid OrderId, ProblemDetails ProblemDetails);