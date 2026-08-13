using EcomDemo.Application.Abstractions;
using EcomDemo.Domain.Accounts;
using System.Security.Cryptography;
using System.Text;

namespace EcomDemo.Infrastructure.Persistence;

/// <summary>Demo credentials: demo@example.com / Demo123! (SHA256 for demo only; production: ASP.NET Identity + bcrypt).</summary>
public sealed class InMemoryUserStore : IUserStore
{
    private static readonly List<User> _users =
    [
        new(Guid.NewGuid(), "demo@example.com", Hash("Demo123!"))
    ];

    public Task<User?> FindByEmailAsync(string email, CancellationToken ct) =>
        Task.FromResult(_users.FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));

    public bool VerifyPassword(User user, string password) =>
        user.PasswordHash == Hash(password);

    private static string Hash(string password) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes("demo-salt:" + password)));
}