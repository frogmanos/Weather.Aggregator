using Polly;
using Polly.Extensions.Http;
using Weather.Aggregator.Application.Logging;

namespace Weather.Aggregator.Infrastructure.Http.Client.ResiliencePolicy;

internal static class GeneralResiliencePolicy
{
    private const string PolicyKey = nameof(GeneralResiliencePolicy);
    public static IAsyncPolicy<HttpResponseMessage> Create<TClient>(ILogging<TClient> logging) where TClient : class
    {        
        return HttpPolicyExtensions
            .HandleTransientHttpError()             
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), 
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    logging.Warn(
                        message: $"{typeof(TClient).Name}.ResiliencePolicy",
                        additionalData: new Dictionary<string, object>
                        {
                            ["RetryAttempt"] = retryAttempt,
                            ["WaitDuration"] = timespan.TotalSeconds,
                            ["StatusCode"] = outcome.Result?.StatusCode.ToString(),
                            ["ExceptionMessage"] = outcome.Exception?.Message ?? "Unknown",
                        });                    
                })
            .WithPolicyKey(PolicyKey);        
    }
}