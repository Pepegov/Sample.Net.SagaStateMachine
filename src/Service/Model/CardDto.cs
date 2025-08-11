namespace Service.Model;

public class CardDto
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required string PanBlock { get; init; }
}