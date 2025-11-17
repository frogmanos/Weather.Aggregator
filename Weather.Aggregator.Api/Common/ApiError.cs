using ErrorTypeEnum = Weather.Aggregator.Application.Common.ErrorType;

namespace Weather.Aggregator.Api.Common;

public record ApiError(string Code, string Message, string ErrorType)
{
    public static readonly ApiError NullValue = new("Null", "Null value was provided", ErrorTypeEnum.Technical.ToString());
}
