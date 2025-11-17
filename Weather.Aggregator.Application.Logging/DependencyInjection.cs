using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Weather.Aggregator.Application.Logging;

public static class DependencyInjection
{
    public static IServiceCollection AddLoggingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICorrelationIdContext, CorrelationIdContext>();
        services.AddSingleton(typeof(ILogging<>), typeof(Logging<>));
        
        return services;
    }

    public static IHostBuilder AddLogging(this IHostBuilder builder)
    {
        builder.ConfigureLogging(l => l.ClearProviders());

        builder.UseSerilog((ctx, services, loggerConfig) =>
        {
            loggerConfig
                .ReadFrom.Configuration(ctx.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName();
            
        });
        return builder;
    }
}