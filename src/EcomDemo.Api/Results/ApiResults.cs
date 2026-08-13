using EcomDemo.Application.Abstractions;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace EcomDemo.Api.Results;

public static class ApiResults
{
    public static IResult ToApiResult<T>(this Result<T> result, int successStatusCode = StatusCodes.Status200OK) =>
        result.IsSuccess
            ? HttpResults.Json(result.Value, statusCode: successStatusCode)
            : result.Error!.Type switch
            {
                ErrorType.NotFound => HttpResults.Problem(
                    title: result.Error.Code, detail: result.Error.Message,
                    statusCode: StatusCodes.Status404NotFound),
                ErrorType.Conflict => HttpResults.Problem(
                    title: result.Error.Code, detail: result.Error.Message,
                    statusCode: StatusCodes.Status409Conflict),
                ErrorType.Unauthorized => HttpResults.Problem(
                    title: result.Error.Code, detail: result.Error.Message,
                    statusCode: StatusCodes.Status401Unauthorized),
                _ => HttpResults.Problem(
                    title: result.Error.Code, detail: result.Error.Message,
                    statusCode: StatusCodes.Status400BadRequest)
            };
}