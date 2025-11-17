using Weather.Aggregator.Application.Common;
using Weather.Aggregator.Application.DataTransferObjects.FetchWeather;

namespace Weather.Aggregator.Application.Interfaces.Services;

public interface IFetchWeatherService 
{
    Task<Result<FetchWeatherResult>> ExecuteAsync(FetchWeatherQuery request, CancellationToken cancellationToken);
}