namespace Choreography.Contracts.Order;

public record OrderCreateEventFailed(Guid OrderId, ProblemDetails ProblemDetails);