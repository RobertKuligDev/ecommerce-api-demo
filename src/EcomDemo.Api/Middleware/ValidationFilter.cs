using FluentValidation;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace EcomDemo.Api.Middleware;

/// <summary>Validates request shape at the boundary; business rules stay in Application.</summary>
public sealed class ValidationFilter<TRequest> : IEndpointFilter where TRequest : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.Arguments.FirstOrDefault(a => a is TRequest) is not TRequest request)
            return HttpResults.BadRequest(new { error = "Missing request body." });

        var validator = context.HttpContext.RequestServices.GetRequiredService<IValidator<TRequest>>();
        var result = await validator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errors = result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            return HttpResults.ValidationProblem(errors);
        }

        return await next(context);
    }
}