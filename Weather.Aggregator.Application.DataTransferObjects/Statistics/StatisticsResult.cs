namespace Weather.Aggregator.Application.DataTransferObjects.Statistics;

public record StatisticsResult(IReadOnlyList<StatisticEntry> Statistics);