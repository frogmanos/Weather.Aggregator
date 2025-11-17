namespace Weather.Aggregator.Contracts.Api.FetchWeather;

public record FetchWeatherResponse
{
    public required string City { get; init; }

    public required IReadOnlyList<CurrentWeather> WeatherSources { get; init; }
}