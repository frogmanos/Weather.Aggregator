namespace Weather.Aggregator.Application.DataTransferObjects.FetchWeather
{
    public record CurrentWeather
    {
        public string SourceName { get; init; } = default!;
        public DateTime TimestampUtc { get; init; }
        public double TemperatureC { get; init; }
        public double? FeelsLikeC { get; init; }
        public double? HumidityPercent { get; init; }
        public string? Condition { get; init; }
    }
}