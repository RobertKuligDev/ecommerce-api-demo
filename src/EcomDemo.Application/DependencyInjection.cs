using EcomDemo.Application.Accounts;
using EcomDemo.Application.Baskets;
using EcomDemo.Application.Products;
using Microsoft.Extensions.DependencyInjection;

namespace EcomDemo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services) =>
        services
            .AddScoped<LoginHandler>()
            .AddScoped<CreateBasketHandler>()
            .AddScoped<AddBasketItemHandler>()
            .AddScoped<GetBasketHandler>()
            .AddScoped<GetProductsHandler>();
}
