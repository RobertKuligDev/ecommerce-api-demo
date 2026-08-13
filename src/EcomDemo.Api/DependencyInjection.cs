using EcomDemo.Api.Endpoints;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EcomDemo.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>(ServiceLifetime.Scoped);
        return services;
    }
}