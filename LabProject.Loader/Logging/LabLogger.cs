namespace LabProject.Loader.Logging;

using Microsoft.Extensions.Logging;
using NorthwoodLib.Pools;

public class LabLogger(string categoryName) : ILogger
{
    private ConsoleColor GetLevelColor(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => ConsoleColor.DarkGray,
            LogLevel.Debug => ConsoleColor.Gray,
            LogLevel.Information => ConsoleColor.White,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Critical => ConsoleColor.DarkRed,
            LogLevel.None => ConsoleColor.White,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };
    }

    private string GetLevelPrefix(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "[TRACE] ",
            LogLevel.Debug => "[DEBUG] ",
            LogLevel.Information => "[INFO] ",
            LogLevel.Warning => "[WARN] ",
            LogLevel.Error => "[ERROR] ",
            LogLevel.Critical => "[FATAL] ",
            LogLevel.None => string.Empty,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!this.IsEnabled(logLevel))
        {
            return;
        }

        var stringBuilder = StringBuilderPool.Shared.Rent();

        var message = formatter(state, exception);

        stringBuilder.Append(message);

        if (exception is not null)
        {
            stringBuilder.AppendLine();
            stringBuilder.Append("Exception: ");
            stringBuilder.Append(exception);
        }

        var output = $"{this.GetLevelPrefix(logLevel)}[{categoryName}] {stringBuilder}";

        ServerConsole.AddLog(output, this.GetLevelColor(logLevel));

        StringBuilderPool.Shared.Return(stringBuilder);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
    {
        return null;
    }
}