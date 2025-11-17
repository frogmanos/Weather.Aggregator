namespace Weather.Aggregator.Application.Common;

public record Error(string Code, string Message, ErrorType ErrorType)
{
    public static readonly Error NullValue = new("Null", "Null value was provided", ErrorType.Technical);   
}