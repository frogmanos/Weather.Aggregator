namespace Weather.Aggregator.Application.Logging
{
    public sealed class CorrelationIdContext : ICorrelationIdContext
    {     
        public string GetCorrelationId => Guid.NewGuid().ToString("D");
    }
}
