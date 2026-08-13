using EcomDemo.Application.Abstractions;

namespace EcomDemo.Application.Products;

public sealed record ProductDto(int Id, string Name, decimal UnitPrice, int Stock);

public sealed class GetProductsHandler(IProductRepository products)
{
    public async Task<Result<IReadOnlyList<ProductDto>>> Handle(CancellationToken ct = default)
    {
        var all = await products.GetAllAsync(ct);
        return Result<IReadOnlyList<ProductDto>>.Success(
            all.Select(p => new ProductDto(p.Id, p.Name, p.UnitPrice, p.Stock)).ToList());
    }
}