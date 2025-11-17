namespace Weather.Aggregator.Application.DataTransferObjects.Statistics
{
    public record StatisticEntry(
        string OperationName,
        long TotalRequests,
        double AverageResponseTimeMs,
        IReadOnlyList<StatisticBucket> StatisticBuckets);
}