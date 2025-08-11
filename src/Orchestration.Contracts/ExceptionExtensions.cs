using System.Net;

namespace Orchestration.Contracts;

public static class ExceptionExtensions
{
    public static ProblemDetails ToProblemDetails(this Exception e, int status, string? instance = null)
    {
        var type = e.GetType();
        return new ProblemDetails()
        {
            Type = type.FullName,
            Title = type.Name,
            Status = status,
            Details = e.Message,
            Instance = instance
        };
    }
    
    public static ProblemDetails ToProblemDetails(this Exception e, HttpStatusCode status, string? instance = null)
    {
        var type = e.GetType();
        return new ProblemDetails()
        {
            Type = type.FullName,
            Title = type.Name,
            Status = (int)status,
            Details = e.Message,
            Instance = instance
        };
    }
}