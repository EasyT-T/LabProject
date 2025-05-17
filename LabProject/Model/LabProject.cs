namespace LabProject.Model;

using System;
using System.Diagnostics.CodeAnalysis;
using global::LabProject.Logging;

public sealed class LabProject
{
    [MemberNotNullWhen(true, nameof(instance))]
    public static bool InstanceSet { get; private set; }

    public static LabProject Instance
    {
        get
        {
            if (!InstanceSet)
            {
                throw new NullReferenceException("The instance has not been set yet.");
            }

            return instance;
        }
    }

    private static LabProject? instance;

    public string PluginName { get; }
    public string LoggingPath { get; }

    internal LoggerFactory InternalLoggerFactory { get; }
    internal ILogger InternalLogger { get; }
    internal ILogger InternalPrintOnlyLogger { get; }

    private LabProject(string pluginName, string? loggingPath)
    {
        this.PluginName = pluginName;
        this.LoggingPath = loggingPath ?? string.Empty;

        this.InternalLoggerFactory =
            string.IsNullOrEmpty(this.LoggingPath) ? new LoggerFactory() : new LoggerFactory(this.LoggingPath);
        this.InternalLogger = this.InternalLoggerFactory.CreateLogger(this.PluginName);
        this.InternalPrintOnlyLogger = new ConsoleLogger($"{pluginName} PRINT ONLY");
    }

    internal static void Create(string pluginName, string? loggingPath)
    {
        if (InstanceSet)
        {
            throw new InvalidOperationException("The instance has already been set.");
        }

        instance = new LabProject(pluginName, loggingPath);
        InstanceSet = true;
    }

    internal static void Destroy()
    {
        if (!InstanceSet)
        {
            throw new InvalidOperationException("The instance has not been set yet.");
        }

        instance = null;
        InstanceSet = false;
    }
}