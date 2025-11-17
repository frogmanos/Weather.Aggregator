using Weather.Aggregator.Application.Common;
using Weather.Aggregator.Application.DataTransferObjects.FetchWeather;

namespace Weather.Aggregator.Application.Interfaces.Api;

public interface IWeatherClient
{
    Task<Result<CurrentWeather>> GetWeatherDataAsync(double latitude, double longitude, CancellationToken cancellationToken);
}