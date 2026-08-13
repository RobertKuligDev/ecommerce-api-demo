using EcomDemo.Api.Middleware;
using EcomDemo.Api.Results;
using EcomDemo.Application.Baskets;
using FluentValidation;

namespace EcomDemo.Api.Endpoints;

public sealed record AddBasketItemRequest(int ProductId, int Quantity);

public sealed class AddBasketItemRequestValidator : AbstractValidator<AddBasketItemRequest>
{
    public AddBasketItemRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.Quantity).InclusiveBetween(1, 100)
            .WithMessage("Quantity must be between 1 and 100 units.");
    }
}

public static class BasketEndpoints
{
    public static RouteGroupBuilder MapBaskets(this RouteGroupBuilder group)
    {
        var baskets = group.MapGroup("/baskets").RequireAuthorization();

        baskets.MapPost("/", async (CreateBasketHandler handler, CancellationToken ct) =>
            (await handler.Handle(ct)).ToApiResult(StatusCodes.Status201Created));

        baskets.MapGet("/{basketId:guid}", async (
                Guid basketId, GetBasketHandler handler, CancellationToken ct) =>
            (await handler.Handle(basketId, ct)).ToApiResult());

        baskets.MapPost("/{basketId:guid}/items", async (
                Guid basketId,
                AddBasketItemRequest request,
                AddBasketItemHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.Handle(
                    new AddBasketItemCommand(basketId, request.ProductId, request.Quantity), ct);
                return result.ToApiResult(StatusCodes.Status201Created);
            })
            .AddEndpointFilter<ValidationFilter<AddBasketItemRequest>>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return group;
    }
}