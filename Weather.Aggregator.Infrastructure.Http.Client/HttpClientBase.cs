using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Weather.Aggregator.Application.Interfaces.Services;
using Weather.Aggregator.Application.Logging;

namespace Weather.Aggregator.Infrastructure.Http.Client;

public abstract class HttpClientBase(
    IHttpClientFactory httpClientFactory,   
    IBenchmarkService benchmarkService)   
{
    public abstract string Name { get; }
  
    protected async Task<HttpResponseMessage> PerformGetActionAsync(
        string requestUrl, 
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(Name);

        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await benchmarkService.Benchmark(Name,
             () => httpClient.GetAsync(requestUrl, cancellationToken));

        return response;
    }
}