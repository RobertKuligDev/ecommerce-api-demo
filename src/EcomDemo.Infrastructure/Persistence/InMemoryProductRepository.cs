using EcomDemo.Application.Abstractions;
using EcomDemo.Domain.Products;

namespace EcomDemo.Infrastructure.Persistence;

public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _catalog =
    [
        new(1, "Winter Jacket Premium", 349.99m, 12),
        new(2, "Wool Beanie", 49.99m, 40),
        new(3, "Leather Gloves", 89.99m, 25)
    ];

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<Product>>(_catalog);

    public Task<Product?> FindByIdAsync(int id, CancellationToken ct) =>
        Task.FromResult(_catalog.FirstOrDefault(p => p.Id == id));
}