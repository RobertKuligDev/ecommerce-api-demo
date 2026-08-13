using EcomDemo.Application.Abstractions;
using EcomDemo.Application.Accounts;
using EcomDemo.Domain.Accounts;
using FluentAssertions;
using System.Security.Claims;

namespace EcomDemo.Tests.Accounts;

public sealed class FakeUserStore : IUserStore
{
    private readonly Dictionary<string, User> _users = [];
    private readonly Dictionary<string, string> _passwords = [];

    public void Seed(User user, string plainPassword)
    {
        _users[user.Email] = user;
        _passwords[user.Email] = plainPassword;
    }

    public Task<User?> FindByEmailAsync(string email, CancellationToken ct) =>
        Task.FromResult(_users.GetValueOrDefault(email));

    public bool VerifyPassword(User user, string password) =>
        _passwords.TryGetValue(user.Email, out var expected) && expected == password;
}

public sealed class FakeTokenService : ITokenService
{
    public string IssueToken(string subject, IEnumerable<Claim> claims) => $"fake-jwt-{subject}";
}

public class LoginHandlerTests
{
    private readonly FakeUserStore _users = new();
    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        _users.Seed(new User(Guid.NewGuid(), "alice@example.com", "hashed"), "correct-password");
        _handler = new LoginHandler(new FakeTokenService(), _users);
    }

    [Fact]
    public async Task Returns_token_on_valid_credentials()
    {
        var result = await _handler.Handle(
            new LoginCommand("alice@example.com", "correct-password"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().Be("fake-jwt-alice@example.com");
        result.Value.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Returns_unauthorized_on_wrong_password()
    {
        var result = await _handler.Handle(
            new LoginCommand("alice@example.com", "wrong-password"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Unauthorized);
        result.Error.Code.Should().Be("auth.invalid_credentials");
    }

    [Fact]
    public async Task Returns_unauthorized_on_unknown_email()
    {
        var result = await _handler.Handle(
            new LoginCommand("nobody@example.com", "any"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Unauthorized);
        result.Error.Code.Should().Be("auth.invalid_credentials");
    }
}