namespace Weather.Aggregator.Application.Common;

public enum ErrorType
{
    None = 0,
    Validation,
    Business,
    NotFound,
    Technical
}