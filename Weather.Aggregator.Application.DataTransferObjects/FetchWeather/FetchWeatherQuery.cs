namespace Weather.Aggregator.Application.DataTransferObjects.FetchWeather
{
    public record FetchWeatherQuery(string City, string? SortBy);
}