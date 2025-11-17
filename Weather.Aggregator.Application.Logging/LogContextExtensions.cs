using Serilog.Context;

namespace Weather.Aggregator.Application.Logging;

internal static class LogContextExtensions
{
    public static IDisposable PushProperties(IDictionary<string, object> properties)
    {
        var disposables = properties
            .Select(kv => LogContext.PushProperty(kv.Key, kv.Value))
            .ToList();

        return new CompositeDisposable(disposables);
    }

    private class CompositeDisposable : IDisposable
    {
        private readonly IEnumerable<IDisposable> _disposables;
        public CompositeDisposable(IEnumerable<IDisposable> disposables) => _disposables = disposables;
        public void Dispose()
        {
            foreach (var d in _disposables) d.Dispose();
        }
    }
}