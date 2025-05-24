namespace LabProject.Logging;

public class WrappedLogger(params ILogger[] loggers) : ILogger
{
    public void Log(string message, LogLevel level)
    {
        loggers.ForEach(l => l.Log(message, level));
    }

    public void Debug(string message)
    {
        loggers.ForEach(x => x.Debug(message));
    }

    public void Info(string message)
    {
        loggers.ForEach(x => x.Info(message));
    }

    public void Warn(string message)
    {
        loggers.ForEach(x => x.Warn(message));
    }

    public void Error(string message)
    {
        loggers.ForEach(x => x.Error(message));
    }
}