using System.Diagnostics.CodeAnalysis;

namespace Weather.Aggregator.Application.Common;

public class Result
{
    public bool IsSuccess { get; }
    public List<Error> Errors { get; } = [];

    public bool HasErrors => Errors.Count > 0;

    public Result(bool isSuccess, IEnumerable<Error>? errors = null)
    {
        IsSuccess = isSuccess;
        if (errors != null)
            Errors.AddRange(errors);
    }

    public static Result Success() => new(true);

    public static Result Failure(Error[] errors) =>
        new(false, errors);

    public static Result Failure(Error error) =>
        new(false, [error]);

    public static Result<T> Success<T>(T value) =>
        new(true, value, []);

    public static Result<T> Failure<T>(Error[] errors) =>
        new(false, default, errors);

    public static Result<T> Failure<T>(Error error) =>
        new(false, default, [error]);
}

public class Result<T>(
    bool isSuccess, 
    T? value, 
    IEnumerable<Error>? errors) : Result(isSuccess, errors)
{
    [NotNull]
    public T Value => IsSuccess
        ? value!
        : throw new InvalidOperationException("The value of a failure result can't be accessed.");

    public static implicit operator Result<T>(T? value) =>
        value is not null ? Success(value) : Failure<T>(Error.NullValue);
}