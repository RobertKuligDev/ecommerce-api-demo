using EcomDemo.Application.Abstractions;

namespace EcomDemo.Application.Baskets;

public sealed class GetBasketHandler(IBasketRepository baskets, IProductRepository products)
{
    public async Task<Result<BasketDto>> Handle(Guid basketId, CancellationToken ct = default)
    {
        var basket = await baskets.FindByIdAsync(basketId, ct);
        if (basket is null)
            return Result<BasketDto>.Failure(BasketErrors.NotFound(basketId));

        var catalog = await products.GetAllAsync(ct);
        var lines = basket.Items
            .Select(i =>
            {
                var p = catalog.Single(c => c.Id == i.ProductId);
                return new BasketLineDto(p.Id, p.Name, p.UnitPrice, i.Quantity, i.Quantity * p.UnitPrice);
            })
            .ToList();

        return Result<BasketDto>.Success(new BasketDto(basket.Id, lines, lines.Sum(l => l.LineTotal)));
    }
}