using Weather.Aggregator.Application.DataTransferObjects.Statistics;

namespace Weather.Aggregator.Application.Interfaces.Services;

public interface IBenchmarkService
{
    Task<TResult> Benchmark<TResult>(string operationName, Func<Task<TResult>> action) where TResult : class;

    IReadOnlyDictionary<string, BenchmarkResult> GetBenchmarkResults();
}