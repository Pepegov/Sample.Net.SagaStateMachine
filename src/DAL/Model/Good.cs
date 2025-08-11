namespace DAL.Model;

public class Good
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public int Count { get; set; }
}