using Weather.Aggregator.Application.Common;
using Weather.Aggregator.Application.Interfaces.Providers;
using Weather.Aggregator.Application.Interfaces.Services;

namespace Weather.Aggregator.Application.Services.Helper;

public sealed class HelperService (ICoordinatesProvider coordinatesProvider) : IHelperService
{
    public Task<Result<IReadOnlyList<string>>> GetCitiesAsync()
        => Task.FromResult(Result.Success(coordinatesProvider.GetCitiesAsync()));
}