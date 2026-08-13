using EcomDemo.Application.Abstractions;
using EcomDemo.Infrastructure.Auth;
using EcomDemo.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace EcomDemo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services) =>
        services
            .AddScoped<IBasketRepository, InMemoryBasketRepository>()
            .AddScoped<IProductRepository, InMemoryProductRepository>()
            .AddScoped<IUserStore, InMemoryUserStore>()
            .AddScoped<ITokenService, JwtTokenService>();
}