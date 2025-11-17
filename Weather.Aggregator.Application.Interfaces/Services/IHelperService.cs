using Weather.Aggregator.Application.Common;

namespace Weather.Aggregator.Application.Interfaces.Services;

public interface IHelperService
{
    Task<Result<IReadOnlyList<string>>> GetCitiesAsync();
}