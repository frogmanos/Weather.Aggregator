namespace Weather.Aggregator.Api.Mappers;

public static class FetchWeatherMapper
{
    public static Contracts.Api.FetchWeather.FetchWeatherResponse ToContract(this Application.DataTransferObjects.FetchWeather.FetchWeatherResult dto) =>
        new()
        {
            City = dto.City,
            WeatherSources = dto.WeatherSources.Select(ws => ws.ToContract()).ToList()            
        };

    private static Contracts.Api.FetchWeather.CurrentWeather ToContract(this Application.DataTransferObjects.FetchWeather.CurrentWeather dto) =>
        new()
        {
            SourceName = dto.SourceName,
            TimestampUtc = dto.TimestampUtc,
            TemperatureC = dto.TemperatureC,
            FeelsLikeC = dto.FeelsLikeC,
            HumidityPercent = dto.HumidityPercent,
            Condition = dto.Condition
        };
}