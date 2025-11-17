using System.Text.Json.Serialization;

namespace Weather.Aggregator.Infrastructure.Providers;

internal record CoordinateEntry
{
    [JsonPropertyName("lat")]
    public required double Latitude { get; init; }

    [JsonPropertyName("lon")]
    public required double Longitude { get; init; }

    [JsonPropertyName("city")]
    public required string City { get; init; }
}