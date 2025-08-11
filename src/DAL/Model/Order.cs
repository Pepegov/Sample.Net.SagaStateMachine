using DAL.Enums;

namespace DAL.Model;

public class Order
{
    public required Guid Id { get; init; }
    public required DateTime CreationAtUtc { get; init; }
    public required List<string> CartItems { get; init; }
    public required decimal Amount { get; init; }
    public required Guid UserId { get; init; }
    public required string DeliveryAddress { get; init; }
    public required OrderStatus Status { get; init; }
}