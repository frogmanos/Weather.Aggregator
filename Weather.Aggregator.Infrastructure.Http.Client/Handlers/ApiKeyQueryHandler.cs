namespace Weather.Aggregator.Infrastructure.Http.Client.Handlers;

public sealed class ApiKeyQueryHandler(string parameterName, string apiKey) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.Method != HttpMethod.Get)
        {
            return base.SendAsync(request, cancellationToken);
        }

        var uri = request.RequestUri!;

        var uriBuilder = new UriBuilder(uri);

        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
      
        query[parameterName] = apiKey;

        uriBuilder.Query = query.ToString();

        request.RequestUri = uriBuilder.Uri;

        return base.SendAsync(request, cancellationToken);
    }
}