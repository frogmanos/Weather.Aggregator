namespace Weather.Aggregator.Application.Logging;

public interface ICorrelationIdContext
{
    public string GetCorrelationId { get; }
}