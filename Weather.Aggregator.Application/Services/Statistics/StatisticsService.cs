using Weather.Aggregator.Application.DataTransferObjects.Statistics;
using Weather.Aggregator.Application.Interfaces.Services;

namespace Weather.Aggregator.Application.Services.Statistics;

public sealed class StatisticsService(IBenchmarkService benchmarkService) : IStatisticsService
{  
    public Task<StatisticsResult> GetStatisticsAsync()
    {
        var benchmarkResult = benchmarkService.GetBenchmarkResults();

        return Task.FromResult(new StatisticsResult(
            benchmarkResult
                .Select(result => new StatisticEntry(
                    result.Key,
                    result.Value.TotalRequests,
                    new TimeSpan(result.Value.TotalDurationTicks).TotalMilliseconds / result.Value.TotalRequests,
                    [
                        new StatisticBucket("Fast(<100ms)", (int)result.Value.FastCount),
                        new StatisticBucket("Average(100-200ms)", (int)result.Value.AverageCount),
                        new StatisticBucket("Slow(>200ms)", (int)result.Value.SlowCount)
                    ]
                ))
                .ToList()));
    }

}