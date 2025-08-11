namespace Service.Model;

public class OrderCreationModel
{
    public required Guid UserId { get; init; }
    public required string DeliveryAddress { get; init; }
    public required List<string> CartItems { get; init; }
    public required decimal Amount { get; init; }
}