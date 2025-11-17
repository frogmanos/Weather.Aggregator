using System.Runtime.CompilerServices;

namespace Weather.Aggregator.Application.Logging;

public interface ILogging<T> where T : class
{
    IDisposable BeginScope(IDictionary<string, object> properties);
    IDisposable BeginScope(string propertyName, object value);

    void Info(string message,
        IDictionary<string, object>? additionalData = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0);

    void Error(string message,
        Exception? exception = null,
        IDictionary<string, object>? additionalData = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0);

    void Warn(string message,
        IDictionary<string, object>? additionalData = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0);

    void Debug(string message,
        IDictionary<string, object>? additionalData = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0);
}