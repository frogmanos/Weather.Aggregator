using Weather.Aggregator.Application.Common;

namespace Weather.Aggregator.Application.DataTransferObjects.Common
{
    public static class ApplicationErrors
    {
        public static readonly Error WeatherNoDataAvailable = 
            new Error(nameof(WeatherNoDataAvailable), "No weather data available for the specified location", ErrorType.Business);

        public static readonly Error WeatherDataFetchFailed = 
            new Error(nameof(WeatherDataFetchFailed), "Failed response when fetching weather data", ErrorType.Technical);

        public static readonly Error InvalidLocationInput =
            new Error(nameof(InvalidLocationInput), "The provided location input is invalid", ErrorType.Validation);
    }
}
