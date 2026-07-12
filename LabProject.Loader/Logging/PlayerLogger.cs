namespace LabProject.Loader.Logging;

using Microsoft.Extensions.Logging;

public class PlayerLogger(string playerName, ILogger baseLogger) : ILogger
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        baseLogger.Log(logLevel, eventId, state, exception, FormatLog);

        return;

        string FormatLog(TState logState, Exception? logException)
        {
            var message = formatter(logState, logException);

            return $"[{playerName}] {message}";
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return baseLogger.IsEnabled(logLevel);
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
    {
        return baseLogger.BeginScope(state);
    }
}