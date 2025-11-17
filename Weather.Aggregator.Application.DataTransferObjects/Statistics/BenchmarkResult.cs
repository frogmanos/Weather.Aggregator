namespace Weather.Aggregator.Application.DataTransferObjects.Statistics;

public sealed class BenchmarkResult
{
    public long TotalRequests;
    public long TotalDurationTicks;
    public long FastCount;
    public long AverageCount;
    public long SlowCount;

    public BenchmarkResult CreateSnapshot()
    {      
        return new BenchmarkResult {
            TotalRequests = Interlocked.Read(ref TotalRequests),
            TotalDurationTicks = Interlocked.Read(ref TotalDurationTicks),
            FastCount = Interlocked.Read(ref FastCount),
            AverageCount = Interlocked.Read(ref AverageCount),
            SlowCount = Interlocked.Read(ref SlowCount)
        };
    }
}