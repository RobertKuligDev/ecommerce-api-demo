namespace EcomDemo.Domain.Products;

public sealed record Product(int Id, string Name, decimal UnitPrice, int Stock);