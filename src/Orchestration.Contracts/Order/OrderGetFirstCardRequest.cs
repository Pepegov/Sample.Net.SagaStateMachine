namespace Orchestration.Contracts.Order;

/// <summary>
/// Get first user card
/// </summary>
/// <param name="UserId"></param>
/// <returns>MqResult of CardDto</returns>
public record OrderGetFirstCardRequest(Guid UserId);