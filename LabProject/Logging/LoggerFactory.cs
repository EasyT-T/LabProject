namespace LabProject.Logging;

using System.IO;

public class LoggerFactory
{
    public bool FileLoggingEnabled { get; }
    public string FileLoggingPath { get; }

    public LoggerFactory()
    {
        this.FileLoggingEnabled = false;
        this.FileLoggingPath = string.Empty;
    }

    public LoggerFactory(string logPath)
    {
        this.FileLoggingEnabled = true;
        this.FileLoggingPath = logPath;
    }

    public ILogger CreateLogger(string name)
    {
        var consoleLogger = new ConsoleLogger(name);
        var fileLogger = this.FileLoggingEnabled ? new FileLogger(Path.Combine(this.FileLoggingPath, name + ".log"), name) : null;

        return fileLogger == null ? new WrappedLogger(consoleLogger) : new WrappedLogger(consoleLogger, fileLogger);
    }
}