using System.Diagnostics.CodeAnalysis;

namespace Weather.Aggregator.Api.Common;

public class ApiResponse
{
    public bool IsSuccess { get; }
    public List<ApiError> Errors { get; } = [];

    public ApiResponse(bool isSuccess, IEnumerable<ApiError>? errors = null)
    {
        IsSuccess = isSuccess;
        if (errors != null)
            Errors.AddRange(errors);
    }

    public static ApiResponse Success() => new(true);

    public static ApiResponse Failure(ApiError[] errors) =>
        new(false, errors);

    public static ApiResponse Failure(ApiError error) =>
        new(false, [error]);

    public static ApiResponse<T> Success<T>(T value) =>
        new(true, value, []);

    public static ApiResponse<T> Failure<T>(ApiError[] errors) =>
        new(false, default, errors);

    public static ApiResponse<T> Failure<T>(ApiError error) =>
        new(false, default, [error]);
}

public class ApiResponse<T>(
    bool isSuccess, 
    T? value, 
    IEnumerable<ApiError>? errors) 
    : ApiResponse(isSuccess, errors)
{
    public T? Value => value;

    public static implicit operator ApiResponse<T>(T? value) =>
        value is not null ? Success(value) : Failure<T>(ApiError.NullValue);
}