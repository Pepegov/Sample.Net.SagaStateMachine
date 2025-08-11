namespace Orchestration.Contracts;

public class SagaResponse(Guid correlationId, ProblemDetails? problemDetails = null)
{
    public Guid CorrelationId { get; set; } = correlationId;
    public ProblemDetails? ProblemDetails { get; set; } = problemDetails;
}