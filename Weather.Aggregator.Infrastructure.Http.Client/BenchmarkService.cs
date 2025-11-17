using System.Collections.Concurrent;
using System.Diagnostics;
using Weather.Aggregator.Application.DataTransferObjects.Statistics;
using Weather.Aggregator.Application.Interfaces.Services;

namespace Weather.Aggregator.Infrastructure.Http.Client;

public sealed class BenchmarkService: IBenchmarkService
{
    private readonly ConcurrentDictionary<string, BenchmarkResult> _benchmarks = new();

    private const int FastThresholdMs = 100;
    private const int AverageThresholdMs = 200;

    public async Task<TResult> Benchmark<TResult>(
        string operationName,
        Func<Task<TResult>> action) where TResult : class
    {
        var timer = Stopwatch.StartNew();

        var result = await action();

        timer.Stop();

        CalculateStatistics(operationName, timer);

        return result;
    }

    public IReadOnlyDictionary<string, BenchmarkResult> GetBenchmarkResults()
        => _benchmarks.ToDictionary(
            keySelector => keySelector.Key,
            value => value.Value.CreateSnapshot());
   
    private void CalculateStatistics(string operationName, Stopwatch timer) 
    {
        var ms = timer.Elapsed.TotalMilliseconds;

        var benchmarkResult = _benchmarks.GetOrAdd(
            operationName,
            _ => new BenchmarkResult());

        Interlocked.Increment(ref benchmarkResult.TotalRequests);
        Interlocked.Add(ref benchmarkResult.TotalDurationTicks, timer.Elapsed.Ticks);

        if (ms < FastThresholdMs)
            Interlocked.Increment(ref benchmarkResult.FastCount);
        else if (ms < AverageThresholdMs)
            Interlocked.Increment(ref benchmarkResult.AverageCount);
        else
            Interlocked.Increment(ref benchmarkResult.SlowCount);
    }    
}