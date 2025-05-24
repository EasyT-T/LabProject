namespace LabProject.Extension;

using System;
using System.Collections.Generic;
using LabProject.Logging;
using LabProject.Model;

public static class LoggerExtensions
{
    private const int CacheMaxCapacity = 10;
    private static readonly Dictionary<Type, ILogger> CacheLoggers = new Dictionary<Type, ILogger>(CacheMaxCapacity);

    internal static ILogger GetInternalLogger(this ILoggerProvider caller)
    {
        var type = caller.GetType();

        if (!CacheLoggers.TryGetValue(type, out var logger))
        {
            if (CacheLoggers.Count >= CacheMaxCapacity)
            {
                CacheLoggers.Clear();
            }

            logger = LabProject.Instance.InternalLoggerFactory.CreateLogger(type.Name);

            CacheLoggers[type] = logger;
        }

        return logger;
    }
}