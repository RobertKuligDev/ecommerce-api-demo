namespace EcomDemo.Domain.Accounts;

public sealed record User(Guid Id, string Email, string PasswordHash);