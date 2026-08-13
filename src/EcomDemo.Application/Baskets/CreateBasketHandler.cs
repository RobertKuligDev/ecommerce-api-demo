using EcomDemo.Application.Abstractions;
using EcomDemo.Domain.Baskets;

namespace EcomDemo.Application.Baskets;

public sealed class CreateBasketHandler(IBasketRepository baskets)
{
    public async Task<Result<BasketDto>> Handle(CancellationToken ct = default)
    {
        var basket = new Basket(Guid.NewGuid());
        await baskets.SaveAsync(basket, ct);
        return Result<BasketDto>.Success(new BasketDto(basket.Id, [], 0m));
    }
}