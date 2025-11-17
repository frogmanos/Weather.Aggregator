using System.Text.Json;

namespace Weather.Aggregator.Infrastructure.Providers;

internal static class CoordinateDataLoader
{
    internal static IReadOnlyList<CoordinateEntry> LoadCoordinateEntries()
    {
        var coordinatesPath = Path.Combine(AppContext.BaseDirectory, "Coordinates", "coordinates.json");
        if (!File.Exists(coordinatesPath))
        {
            throw new FileNotFoundException("The coordinates.json file was not found.", coordinatesPath);
        }

        var jsonContent = File.ReadAllText(coordinatesPath);
        var coordinateEntries = JsonSerializer.Deserialize<List<CoordinateEntry>>(jsonContent);
        return coordinateEntries ?? [];
    }
}