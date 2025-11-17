using System.Net.Http.Json;
using Weather.Aggregator.Application.Common;
using Weather.Aggregator.Application.DataTransferObjects.Common;
using Weather.Aggregator.Application.DataTransferObjects.FetchWeather;
using Weather.Aggregator.Application.Interfaces.Api;
using Weather.Aggregator.Application.Interfaces.Services;
using Weather.Aggregator.Application.Logging;

namespace Weather.Aggregator.Infrastructure.Http.Client;

public sealed class OpenWeatherClient(
    IHttpClientFactory httpClientFactory, 
    ILogging<OpenWeatherClient> logging,
    IBenchmarkService benchmarkService) 
    : HttpClientBase(httpClientFactory, benchmarkService), IWeatherClient
{
    public override string Name => nameof(OpenWeatherClient);

    public async Task<Result<CurrentWeather>> GetWeatherDataAsync(double latitude, double longitude, CancellationToken cancellationToken)
    {
        var requestUri = $"data/2.5/weather?lat={latitude}&lon={longitude}&units=metric";

        try
        {
            var httpResponse = await PerformGetActionAsync(requestUri, cancellationToken);
            if (!httpResponse.IsSuccessStatusCode)
            {
                logging.Error("Failed to fetch weather data from OpenWeather API");

                return Result.Failure<CurrentWeather>(ApplicationErrors.WeatherDataFetchFailed);
            }

            var json = await httpResponse.Content.ReadFromJsonAsync<dynamic>(cancellationToken: cancellationToken);
            if (json is null)
            {
                logging.Error("Failed to fetch weather data are empty from OpenWeather API");

                return Result.Failure<CurrentWeather>(ApplicationErrors.WeatherDataFetchFailed);
            }
            
            logging.Info($"{nameof(OpenWeatherClient)}.{nameof(GetWeatherDataAsync)}", new Dictionary<string, object>
            {
                ["ClientName"] = Name,
                ["RequestUrl"] = requestUri,
                ["Response"] = json,
                ["StatusCode"] = httpResponse.StatusCode
            });

            return ParseResponse(json);
        }
        catch ( Exception exception) 
        {
            logging.Error(
                message: $"{Name}.{nameof(GetWeatherDataAsync)}",
                exception: exception,
                additionalData: new Dictionary<string, object>
                {
                    ["ClientName"] = Name,
                    ["RequestUrl"] = requestUri
                });

            return Result.Failure<CurrentWeather>(ApplicationErrors.WeatherDataFetchFailed);
        }        
    }

    private Result<CurrentWeather> ParseResponse(dynamic json)
    {
        return new CurrentWeather
        {
            SourceName = Name,
            TimestampUtc = DateTime.UtcNow,
            TemperatureC = json.GetProperty("main").GetProperty("temp").GetDouble(),
            FeelsLikeC = json.GetProperty("main").GetProperty("feels_like").GetDouble(),
            HumidityPercent = json.GetProperty("main").GetProperty("humidity").GetDouble(),
            Condition = json.GetProperty("weather")[0].GetProperty("description").GetString()
        };
    }
}