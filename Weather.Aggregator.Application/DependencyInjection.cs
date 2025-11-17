using Microsoft.Extensions.DependencyInjection;
using Weather.Aggregator.Application.Interfaces.Services;
using Weather.Aggregator.Application.Services.FetchWeather;
using Weather.Aggregator.Application.Services.Helper;
using Weather.Aggregator.Application.Services.Statistics;

namespace Weather.Aggregator.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IFetchWeatherService, FetchWeatherService>();
        services.AddTransient<IStatisticsService, StatisticsService>();
        services.AddSingleton<IHelperService, HelperService>();
        services.AddMemoryCache();
        return services;
    }
}