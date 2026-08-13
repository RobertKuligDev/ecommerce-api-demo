namespace EcomDemo.Application.Abstractions;

public enum ErrorType { Validation, NotFound, Conflict, Unauthorized }

public sealed record Error(string Code, string Message, ErrorType Type);

/// <summary>Explicit success/failure — no exceptions for business errors.</summary>
public sealed class Result<T>
{
    private readonly T? _value;

    private Result(bool isSuccess, T? value, Error? error)
    {
        IsSuccess = isSuccess;
        _value = value;
        Error = error;
    }

    public bool IsSuccess { get; }
    public Error? Error { get; }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Result is a failure; inspect Error instead.");

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(Error error) => new(false, default, error);
}