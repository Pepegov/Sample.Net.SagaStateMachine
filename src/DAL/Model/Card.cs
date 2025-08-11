namespace DAL.Model;

public class Card
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required string PanBlock { get; init; }
}