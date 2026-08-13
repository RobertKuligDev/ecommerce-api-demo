using EcomDemo.Api.Results;
using EcomDemo.Application.Products;

namespace EcomDemo.Api.Endpoints;

public static class ProductEndpoints
{
    public static RouteGroupBuilder MapProducts(this RouteGroupBuilder group)
    {
        group.MapGet("/products", async (GetProductsHandler handler, CancellationToken ct) =>
            (await handler.Handle(ct)).ToApiResult());

        return group;
    }
}