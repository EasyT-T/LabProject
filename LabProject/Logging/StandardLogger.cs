namespace LabProject.Logging;

using System.Text;
using LabProject.Enum;
using NorthwoodLib.Pools;

public abstract class StandardLogger : ILogger
{
    public abstract string Name { get; }

    private readonly StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();

    ~StandardLogger()
    {
        StringBuilderPool.Shared.Return(this.stringBuilder);
    }

    protected abstract void Output(string text, LogLevel level);

    public virtual void Log(string message, LogLevel level)
    {
        var prefix = level switch
        {
            LogLevel.Debug => "[DEBUG]",
            LogLevel.Info => "[INFO]",
            LogLevel.Warn => "[WARN]",
            LogLevel.Error => "[ERROR]",
            _ => string.Empty,
        };

        this.stringBuilder.Clear();

        this.stringBuilder.Append(prefix);
        this.stringBuilder.Append(" [");
        this.stringBuilder.Append(this.Name);
        this.stringBuilder.Append("] ");
        this.stringBuilder.Append(message);

        this.Output(this.stringBuilder.ToString(), level);
    }

    public virtual void Debug(string message)
    {
        this.Log(message, LogLevel.Debug);
    }

    public virtual void Info(string message)
    {
        this.Log(message, LogLevel.Info);
    }

    public virtual void Warn(string message)
    {
        this.Log(message, LogLevel.Warn);
    }

    public virtual void Error(string message)
    {
        this.Log(message, LogLevel.Error);
    }
}