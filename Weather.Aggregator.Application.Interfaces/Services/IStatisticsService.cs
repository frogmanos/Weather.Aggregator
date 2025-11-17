using Weather.Aggregator.Application.DataTransferObjects.Statistics;

namespace Weather.Aggregator.Application.Interfaces.Services;

public interface IStatisticsService
{
    Task<StatisticsResult> GetStatisticsAsync();
}