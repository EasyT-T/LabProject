namespace LabProject.Logging;

using System;

public class ConsoleLogger(string name) : StandardLogger
{
    public override string Name { get; } = name;

    protected override void Output(string text, LogLevel level)
    {
        var color = level switch
        {
            LogLevel.Debug => ConsoleColor.Gray,
            LogLevel.Info => ConsoleColor.White,
            LogLevel.Warn => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null),
        };

        ServerConsole.AddLog(text, color);
    }
}