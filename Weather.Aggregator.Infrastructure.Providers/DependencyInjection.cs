using Microsoft.Extensions.DependencyInjection;
using Weather.Aggregator.Application.Interfaces.Providers;

namespace Weather.Aggregator.Infrastructure.Providers;

public static class DependencyInjection
{
    public static IServiceCollection AddCoordinatesProvider(this IServiceCollection services)
    {
        services.AddTransient<ICoordinatesProvider, CoordinatesProvider>();
        return services;
    }
}