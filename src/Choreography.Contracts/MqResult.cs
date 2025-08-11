using System.Text.Json.Serialization;

namespace Choreography.Contracts;

public class MqResult<TModel> 
{
    [JsonConstructor]
    public MqResult(TModel model, ProblemDetails? problemDetails = null)
    {
        Model = model;
        ProblemDetails = problemDetails;
    }
    
    public MqResult(ProblemDetails problemDetails)
    {
        ProblemDetails = problemDetails;
    }

    public TModel? Model { get; set; }
    public ProblemDetails? ProblemDetails { get; set; }
}