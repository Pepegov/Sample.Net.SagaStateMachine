namespace Choreography.Contracts;

public class ProblemDetails
{
    public required string Title { get; set; }
    public string? Type { get; set; }
    public required int Status { get; init; }
    public required string Details { get; set; }
    public string? Instance { get; set; }

    public override string ToString()
        => $"{nameof(Title)}:{Title}, {nameof(Status)}:{Status}, {nameof(Details)}:{Details}, {nameof(Instance)}:{Instance}";
}