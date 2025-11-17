using System.Collections.Concurrent;
using Weather.Aggregator.Application.Interfaces.Providers;

namespace Weather.Aggregator.Infrastructure.Providers;

public sealed class CoordinatesProvider: ICoordinatesProvider
{
    private readonly ConcurrentDictionary<string, CoordinateEntry> _coordinates = new();

    public CoordinatesProvider()
    {
        var coordinateEntries = CoordinateDataLoader.LoadCoordinateEntries();
        foreach (var entry in coordinateEntries)
        {
            _coordinates[entry.City.ToLowerInvariant()] = entry;
        }
    }

    public (double Latitude, double Longitude) GetCoordinatesAsync(string city, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return (0.0, 0.0);
        }

        var key = city.ToLowerInvariant();

        return _coordinates.TryGetValue(key, out var entry) ? (entry.Latitude, entry.Longitude) : (0.0, 0.0);
    }

    public IReadOnlyList<string> GetCitiesAsync()
        => _coordinates.Values.Select(e => e.City).ToList();
}