using EcomDemo.Domain.Accounts;
using EcomDemo.Domain.Baskets;
using EcomDemo.Domain.Products;
using System.Security.Claims;

namespace EcomDemo.Application.Abstractions;

public interface IBasketRepository
{
    Task<Basket?> FindByIdAsync(Guid id, CancellationToken ct = default);
    Task SaveAsync(Basket basket, CancellationToken ct = default);
}

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default);
    Task<Product?> FindByIdAsync(int id, CancellationToken ct = default);
}

public interface IUserStore
{
    Task<User?> FindByEmailAsync(string email, CancellationToken ct = default);
    bool VerifyPassword(User user, string password);
}

public interface ITokenService
{
    string IssueToken(string subject, IEnumerable<Claim> claims);
}