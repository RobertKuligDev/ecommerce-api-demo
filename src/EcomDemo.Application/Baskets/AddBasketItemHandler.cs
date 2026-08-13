using EcomDemo.Application.Abstractions;
using EcomDemo.Domain.Baskets; 
using EcomDemo.Domain.Products;

namespace EcomDemo.Application.Baskets;

public sealed record AddBasketItemCommand(Guid BasketId, int ProductId, int Quantity);

public sealed record BasketLineDto(int ProductId, string Name, decimal UnitPrice, int Quantity, decimal LineTotal);
public sealed record BasketDto(Guid Id, IReadOnlyCollection<BasketLineDto> Lines, decimal Total);

public static class BasketErrors
{
    public static Error NotFound(Guid id) =>
        new("basket.not_found", $"Basket {id} does not exist.", ErrorType.NotFound);

    public static Error InsufficientStock(Product product, int requested) =>
        new("basket.insufficient_stock",
            $"Only {product.Stock} unit(s) of '{product.Name}' available, requested {requested}.",
            ErrorType.Conflict);
}

public static class ProductErrors
{
    public static Error NotFound(int id) =>
        new("product.not_found", $"Product {id} does not exist.", ErrorType.NotFound);
}

public sealed class AddBasketItemHandler(
    IBasketRepository baskets,
    IProductRepository products)
{
    public async Task<Result<BasketDto>> Handle(AddBasketItemCommand command, CancellationToken ct = default)
    {
        var basket = await baskets.FindByIdAsync(command.BasketId, ct);
        if (basket is null)
            return Result<BasketDto>.Failure(BasketErrors.NotFound(command.BasketId));

        var product = await products.FindByIdAsync(command.ProductId, ct);
        if (product is null)
            return Result<BasketDto>.Failure(ProductErrors.NotFound(command.ProductId));

        // Business rule: cannot add more than available stock
        if (command.Quantity > product.Stock)
            return Result<BasketDto>.Failure(BasketErrors.InsufficientStock(product, command.Quantity));

        basket.AddItem(product.Id, command.Quantity);
        await baskets.SaveAsync(basket, ct);

        return Result<BasketDto>.Success(await ToDto(basket, ct));
    }

    private async Task<BasketDto> ToDto(Basket basket, CancellationToken ct)
    {
        var catalog = await products.GetAllAsync(ct);

        var lines = basket.Items
            .Select(i =>
            {
                var p = catalog.Single(c => c.Id == i.ProductId);
                return new BasketLineDto(p.Id, p.Name, p.UnitPrice, i.Quantity, i.Quantity * p.UnitPrice);
            })
            .ToList();

        return new BasketDto(basket.Id, lines, lines.Sum(l => l.LineTotal));
    }
}