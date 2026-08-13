using EcomDemo.Application.Abstractions;
using EcomDemo.Domain.Baskets;

namespace EcomDemo.Infrastructure.Persistence;

/// <summary>In-memory persistence for the public demo. Production: EfBasketRepository (private repo, PostgreSQL).</summary>
public sealed class InMemoryBasketRepository : IBasketRepository
{
    // Static so state survives across scoped instances and requests —
    // mirrors production persistence without a real database.
    private static readonly Dictionary<Guid, Basket> _store = [];
    private static readonly object _lock = new();

    public Task<Basket?> FindByIdAsync(Guid id, CancellationToken ct)
    {
        lock (_lock) return Task.FromResult(_store.GetValueOrDefault(id));
    }

    public Task SaveAsync(Basket basket, CancellationToken ct)
    {
        lock (_lock) _store[basket.Id] = basket;
        return Task.CompletedTask;
    }
}