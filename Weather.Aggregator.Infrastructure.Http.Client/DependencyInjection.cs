using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weather.Aggregator.Application.Interfaces.Api;
using Weather.Aggregator.Application.Interfaces.Services;
using Weather.Aggregator.Application.Logging;
using Weather.Aggregator.Infrastructure.Http.Client.Handlers;
using Weather.Aggregator.Infrastructure.Http.Client.ResiliencePolicy;

namespace Weather.Aggregator.Infrastructure.Http.Client
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureHttp(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<OpenWeatherClient>(client =>
                {
                    client.BaseAddress = new Uri("https://api.openweathermap.org/");
                    client.Timeout = TimeSpan.FromSeconds(5);
                })
                .AddHttpMessageHandler(() => new ApiKeyQueryHandler(
                    parameterName: "appid",
                    apiKey: configuration["Weather:OpenWeather:ApiKey"]!))
                .AddPolicyHandler((sp, _) =>
                {
                    var logging = sp.GetRequiredService<ILogging<OpenWeatherClient>>();
                    return GeneralResiliencePolicy.Create(logging);
                });

            services.AddHttpClient<WeatherApiClient>(client =>
                {
                    client.BaseAddress = new Uri("https://api.weatherapi.com/");
                    client.Timeout = TimeSpan.FromSeconds(5);
                })
                .AddHttpMessageHandler(() => new ApiKeyQueryHandler(
                    parameterName: "key",
                    apiKey: configuration["Weather:WeatherApi:ApiKey"]!))
                .AddPolicyHandler((sp, _) =>
                {
                    var logging = sp.GetRequiredService<ILogging<WeatherApiClient>>();
                    return GeneralResiliencePolicy.Create(logging);
                });

            services.AddHttpClient<WeatherBitClient>(client =>
                {
                    client.BaseAddress = new Uri("https://api.weatherbit.io/");
                    client.Timeout = TimeSpan.FromSeconds(5);
                })
                .AddHttpMessageHandler(() => new ApiKeyQueryHandler(
                    parameterName: "key",
                    apiKey: configuration["Weather:WeatherBit:ApiKey"]!))
                .AddPolicyHandler((sp, _) =>
                {
                    var logging = sp.GetRequiredService<ILogging<WeatherBitClient>>();
                    return GeneralResiliencePolicy.Create(logging);
                });

            services.AddTransient<IWeatherClient, OpenWeatherClient>();
            services.AddTransient<IWeatherClient, WeatherApiClient>();
            services.AddTransient<IWeatherClient, WeatherBitClient>();

            services.AddSingleton<IBenchmarkService, BenchmarkService>();
            return services;
        }
    }
}
