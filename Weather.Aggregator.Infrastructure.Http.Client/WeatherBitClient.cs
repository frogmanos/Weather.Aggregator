using System.Net.Http.Json;
using Weather.Aggregator.Application.Common;
using Weather.Aggregator.Application.DataTransferObjects.Common;
using Weather.Aggregator.Application.DataTransferObjects.FetchWeather;
using Weather.Aggregator.Application.Interfaces.Api;
using Weather.Aggregator.Application.Interfaces.Services;
using Weather.Aggregator.Application.Logging;

namespace Weather.Aggregator.Infrastructure.Http.Client;

public sealed class WeatherBitClient(
    IHttpClientFactory httpClientFactory, 
    ILogging<WeatherBitClient> logging,
    IBenchmarkService benchmarkService)
    : HttpClientBase(httpClientFactory, benchmarkService), IWeatherClient
{  
    public override string Name => nameof(WeatherBitClient);

    public async Task<Result<CurrentWeather>> GetWeatherDataAsync(double latitude, double longitude, CancellationToken cancellationToken)
    {
        var requestUri = $"v2.0/current?lat={latitude}&lon={longitude}&units=M";

        try
        {
            var httpResponse = await PerformGetActionAsync(requestUri, cancellationToken);
            if (!httpResponse.IsSuccessStatusCode)
            {
                logging.Error("Failed to fetch weather data from WeatherBit API");

                return Result.Failure<CurrentWeather>(ApplicationErrors.WeatherDataFetchFailed);
            }

            var json = await httpResponse.Content.ReadFromJsonAsync<dynamic>(cancellationToken: cancellationToken);
            if (json is null)
            {
                logging.Error("Failed to fetch weather data are empty from WeatherBit API");

                return Result.Failure<CurrentWeather>(ApplicationErrors.WeatherDataFetchFailed);
            }

            logging.Info($"{nameof(WeatherBitClient)}.{nameof(GetWeatherDataAsync)}", new Dictionary<string, object>
            {
                ["ClientName"] = Name,
                ["RequestUrl"] = requestUri,
                ["Response"] = json,
                ["StatusCode"] = httpResponse.StatusCode
            });

            return ParseResponse(json);
        }
        catch (Exception exception)
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
            TemperatureC = json.GetProperty("data")[0].GetProperty("temp").GetDouble(),
            FeelsLikeC = null,
            HumidityPercent = json.GetProperty("data")[0].GetProperty("rh").GetDouble(),
            Condition = json.GetProperty("data")[0].GetProperty("weather").GetProperty("description").GetString()
        };
    }
}