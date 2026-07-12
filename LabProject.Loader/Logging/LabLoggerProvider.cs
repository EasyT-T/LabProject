namespace LabProject.Loader.Logging;

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

public class LabLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, LabLogger> _loggers = new ConcurrentDictionary<string, LabLogger>();

    public ILogger CreateLogger(string categoryName)
    {
        return this._loggers.GetOrAdd(categoryName, new LabLogger(categoryName));
    }

    public void Dispose()
    {
        this._loggers.Clear();
    }
}