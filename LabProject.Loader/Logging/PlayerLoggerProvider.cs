namespace LabProject.Loader.Logging;

using System.Collections.Concurrent;
using LabProject.Loader.PlayerInjectables;
using Microsoft.Extensions.Logging;

public class PlayerLoggerProvider(PlayerContext playerContext) : ILoggerProvider
{
    private readonly LabLoggerProvider _baseLoggerProvider = new LabLoggerProvider();
    private readonly ConcurrentDictionary<string, PlayerLogger> _loggers = new ConcurrentDictionary<string, PlayerLogger>();

    public ILogger CreateLogger(string categoryName)
    {
        var playerName = playerContext.Player.Nickname;
        var baseLogger = this._baseLoggerProvider.CreateLogger(categoryName);

        return this._loggers.GetOrAdd(categoryName, new PlayerLogger(playerName, baseLogger));
    }

    public void Dispose()
    {
        this._loggers.Clear();
        this._baseLoggerProvider.Dispose();
    }
}