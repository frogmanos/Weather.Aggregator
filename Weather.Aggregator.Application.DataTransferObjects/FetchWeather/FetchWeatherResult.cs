namespace Weather.Aggregator.Application.DataTransferObjects.FetchWeather
{
    public record FetchWeatherResult
    {
        public required string City { get; init; }

        public required IReadOnlyList<CurrentWeather> WeatherSources { get; init; }
    }
}