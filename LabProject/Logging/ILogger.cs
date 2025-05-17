namespace LabProject.Logging;

using LabProject.Enum;

public interface ILogger
{
    void Log(string message, LogLevel level);
    void Debug(string message);
    void Info(string message);
    void Warn(string message);
    void Error(string message);
}