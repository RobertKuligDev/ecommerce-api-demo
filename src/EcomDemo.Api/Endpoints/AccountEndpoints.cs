using EcomDemo.Api.Middleware;
using EcomDemo.Api.Results;
using EcomDemo.Application.Accounts;
using FluentValidation;

namespace EcomDemo.Api.Endpoints;

public sealed record LoginRequest(string Email, string Password);

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

public static class AccountEndpoints
{
    public static RouteGroupBuilder MapAccounts(this RouteGroupBuilder group)
    {
        group.MapPost("/accounts/login", async (
                LoginRequest request,
                LoginHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.Handle(
                    new LoginCommand(request.Email, request.Password), ct);
                return result.ToApiResult();
            })
            .AddEndpointFilter<ValidationFilter<LoginRequest>>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return group;
    }
}