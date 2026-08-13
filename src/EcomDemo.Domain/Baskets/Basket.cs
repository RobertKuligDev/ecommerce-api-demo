namespace EcomDemo.Domain.Baskets;

/// <summary>Aggregate root. In production this maps 1:1 to EF Core + PostgreSQL.</summary>
public sealed class Basket
{
    private readonly List<BasketItem> _items = [];

    public Basket(Guid id) => Id = id;

    public Guid Id { get; }
    public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();

    public void AddItem(int productId, int quantity)
    {
        var existing = _items.SingleOrDefault(i => i.ProductId == productId);

        if (existing is null)
            _items.Add(new BasketItem(productId, quantity));
        else
            existing.IncreaseQuantity(quantity);
    }
}

public sealed class BasketItem
{
    public BasketItem(int productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }

    public int ProductId { get; }
    public int Quantity { get; private set; }

    public void IncreaseQuantity(int quantity) => Quantity += quantity;
}