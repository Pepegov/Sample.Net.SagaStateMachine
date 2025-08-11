namespace Service.Model;

public static class Constans
{
    public static readonly Guid UserId = Guid.Parse("b185922e-3061-49a1-a9e6-28521eeca2f9");

    public static readonly GoodViewModel Good = new()
    {
        Id = Guid.Parse("cf7c1502-a22b-4d0a-8f95-5b802e2f7948"),
        Name = "Product",
        Count = 1,
        Price = 100,
    };
}