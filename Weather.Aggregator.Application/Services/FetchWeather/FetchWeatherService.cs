using Microsoft.Extensions.Caching.Memory;
using Weather.Aggregator.Application.Common;
using Weather.Aggregator.Application.DataTransferObjects.Common;
using Weather.Aggregator.Application.DataTransferObjects.FetchWeather;
using Weather.Aggregator.Application.Interfaces.Api;
using Weather.Aggregator.Application.Interfaces.Providers;
using Weather.Aggregator.Application.Interfaces.Services;
using Weather.Aggregator.Application.Logging;

namespace Weather.Aggregator.Application.Services.FetchWeather;

public sealed class FetchWeatherService(
    IEnumerable<IWeatherClient> weatherClients,
    ICoordinatesProvider coordinateProvider,
    ILogging<FetchWeatherService> logging,   
    IMemoryCache memoryCache) : IFetchWeatherService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);

    public async Task<Result<FetchWeatherResult>> ExecuteAsync(FetchWeatherQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"weather:{request.City.ToLowerInvariant()}";
        if (memoryCache.TryGetValue<FetchWeatherResult>(cacheKey, out var cachedResult))
        {
            logging.Info("Cache hit for weather data", new Dictionary<string, object> { { "City", request.City } });
        
            return Result.Success(ApplySort(cachedResult!, request.SortBy));
        }

        var coordinates = coordinateProvider.GetCoordinatesAsync(request.City, cancellationToken);
        if (coordinates.Longitude == 0 || coordinates.Latitude == 0)
        {
            return Result.Failure<FetchWeatherResult>(ApplicationErrors.InvalidLocationInput);
        }

        var tasks = weatherClients
            .Select(client => client.GetWeatherDataAsync(coordinates.Latitude, coordinates.Longitude, cancellationToken))
            .ToList();

        var weatherDataResults = await Task.WhenAll(tasks);

        var successWeatherResults = weatherDataResults
            .Where(result => result.IsSuccess)
            .Select(result => result.Value)
            .ToList();

        var failedWeatherResults = weatherDataResults
            .Where(result => !result.IsSuccess)            
            .ToList();

        if (failedWeatherResults.Count > 0)
        {
            logging.Warn("Failed to fetch weather data from all of the sources",                 
                new Dictionary<string, object>
                {
                    { "FailedErrors", failedWeatherResults.Select(f => f.Errors).ToList() },
                    { "City", request.City },
                    { "Latitude", coordinates.Latitude },
                    { "Longitude", coordinates.Longitude }
                });
        }

        if (successWeatherResults.Count == 0)
        {
            return Result.Failure<FetchWeatherResult>(ApplicationErrors.WeatherNoDataAvailable);
        }

        var fetchWeatherResult = new FetchWeatherResult
        {
            City = request.City,
            WeatherSources = successWeatherResults
        };

        memoryCache.Set(cacheKey, fetchWeatherResult, CacheDuration);

        return Result.Success(ApplySort(fetchWeatherResult, request.SortBy));
    }

    private static FetchWeatherResult ApplySort(FetchWeatherResult fetchWeatherResult, string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return fetchWeatherResult;
        }

        var sortedSources = sortBy.ToLowerInvariant() switch
        {
            "temp" or "temperature" => fetchWeatherResult.WeatherSources.OrderBy(s => s.TemperatureC).ToList(),
            "temp_desc" => fetchWeatherResult.WeatherSources.OrderByDescending(s => s.TemperatureC).ToList(),
            "source" => fetchWeatherResult.WeatherSources.OrderBy(s => s.SourceName).ToList(),
            _ => fetchWeatherResult.WeatherSources
        };

        return fetchWeatherResult with { WeatherSources = sortedSources };
    }
}