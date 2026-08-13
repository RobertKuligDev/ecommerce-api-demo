using EcomDemo.Application.Abstractions;
using EcomDemo.Application.Baskets;
using EcomDemo.Domain.Baskets;
using EcomDemo.Domain.Products;
using FluentAssertions;

namespace EcomDemo.Tests.Baskets;

public sealed class FakeBasketRepository : IBasketRepository
{
    private readonly Dictionary<Guid, Basket> _store = [];
    
    public Basket Seed(Basket basket)
    {
        _store[basket.Id] = basket;
        return basket;
    }

    public Task<Basket?> FindByIdAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(_store.GetValueOrDefault(id));

    public Task SaveAsync(Basket basket, CancellationToken ct)
    {
        _store[basket.Id] = basket;
        return Task.CompletedTask;
    }
}

public sealed class FakeProductRepository : IProductRepository
{
    private readonly List<Product> _catalog = [];

    public void Seed(Product product) => _catalog.Add(product);

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<Product>>(_catalog);

    public Task<Product?> FindByIdAsync(int id, CancellationToken ct) =>
        Task.FromResult(_catalog.FirstOrDefault(p => p.Id == id));
}

public class AddBasketItemHandlerTests
{
    private readonly FakeBasketRepository _baskets = new();
    private readonly FakeProductRepository _products = new();
    private readonly AddBasketItemHandler _handler;

    public AddBasketItemHandlerTests()
    {
        _products.Seed(new Product(1, "Winter Jacket Premium", 349.99m, 12));
        _handler = new AddBasketItemHandler(_baskets, _products);
    }

    [Fact]
    public async Task Adds_item_to_empty_basket()
    {
        var basket = _baskets.Seed(new Basket(Guid.NewGuid()));

        var result = await _handler.Handle(
            new AddBasketItemCommand(basket.Id, 1, 2), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(basket.Id);
        result.Value.Lines.Should().HaveCount(1);
        result.Value.Lines.First().ProductId.Should().Be(1);
        result.Value.Lines.First().Quantity.Should().Be(2);
        result.Value.Lines.First().LineTotal.Should().Be(699.98m);
        result.Value.Total.Should().Be(699.98m);
    }

    [Fact]
    public async Task Returns_not_found_when_basket_missing()
    {
        var result = await _handler.Handle(
            new AddBasketItemCommand(Guid.NewGuid(), 1, 1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
        result.Error.Code.Should().Be("basket.not_found");
    }

    [Fact]
    public async Task Returns_not_found_when_product_missing()
    {
        var basket = _baskets.Seed(new Basket(Guid.NewGuid()));

        var result = await _handler.Handle(
            new AddBasketItemCommand(basket.Id, 999, 1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
        result.Error.Code.Should().Be("product.not_found");
    }

    [Fact]
    public async Task Returns_conflict_when_quantity_exceeds_stock()
    {
        var basket = _baskets.Seed(new Basket(Guid.NewGuid()));

        var result = await _handler.Handle(
            new AddBasketItemCommand(basket.Id, 1, 15), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Conflict);
        result.Error.Code.Should().Be("basket.insufficient_stock");
    }
}