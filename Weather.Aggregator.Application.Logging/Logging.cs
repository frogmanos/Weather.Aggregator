using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Context;
using Serilog.Events;

namespace Weather.Aggregator.Application.Logging;

public sealed class Logging<T>(ILogger logger) : ILogging<T>
    where T : class
{
    private readonly ILogger _logger = logger.ForContext<T>();

    public IDisposable BeginScope(IDictionary<string, object> properties)
        => LogContextExtensions.PushProperties(properties);

    public IDisposable BeginScope(string propertyName, object value)
        => LogContext.PushProperty(propertyName, value, true);

    public void Info(string message,
        IDictionary<string, object>? additionalData = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        LogInternal(LogEventLevel.Information, message, null, additionalData, memberName, sourceFilePath, sourceLineNumber);
    }

    public void Error(string message,
        Exception? exception = null,
        IDictionary<string, object>? additionalData = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        LogInternal(LogEventLevel.Error, message, exception, additionalData, memberName, sourceFilePath, sourceLineNumber);
    }

    public void Warn(string message,
        IDictionary<string, object>? additionalData = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        LogInternal(LogEventLevel.Warning, message, null, additionalData, memberName, sourceFilePath, sourceLineNumber);
    }

    public void Debug(string message,
        IDictionary<string, object>? additionalData = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        LogInternal(LogEventLevel.Debug, message, null, additionalData, memberName, sourceFilePath, sourceLineNumber);
    }

    private void LogInternal(
        LogEventLevel level,
        string message,
        Exception? exception,
        IDictionary<string, object>? additionalData,
        string memberName,
        string sourceFilePath,
        int sourceLineNumber)
    {
        var className = Path.GetFileNameWithoutExtension(sourceFilePath ?? string.Empty);

        var logger = _logger
            .ForContext("Class", className)
            .ForContext("Method", memberName)
            .ForContext("SourceFilePath", sourceFilePath)
            .ForContext("SourceLineNumber", sourceLineNumber);

        if (additionalData is { Count: > 0 })
        {
            logger = additionalData.Aggregate(logger, (current, data) => current.ForContext(data.Key, data.Value, true));
        }
        
        logger.Write(level, exception, message);
    }
}