using EcomDemo.Application.Abstractions;
using System.Security.Claims;

namespace EcomDemo.Application.Accounts;

public sealed record LoginCommand(string Email, string Password);
public sealed record LoginResponse(string Token, DateTime ExpiresAt);

public static class AuthErrors
{
    public static Error InvalidCredentials =>
        new("auth.invalid_credentials", "Email or password is incorrect.", ErrorType.Unauthorized);
}

public sealed class LoginHandler(ITokenService tokens, IUserStore users)
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken ct = default)
    {
        var user = await users.FindByEmailAsync(command.Email, ct);

        if (user is null || !users.VerifyPassword(user, command.Password))
            return Result<LoginResponse>.Failure(AuthErrors.InvalidCredentials);

        var expiresAt = DateTime.UtcNow.AddHours(1);
        var token = tokens.IssueToken(user.Email, new[]
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("role", "customer")
        });

        return Result<LoginResponse>.Success(new LoginResponse(token, expiresAt));
    }
}