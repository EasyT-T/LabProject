namespace LabProject.Logging;

using System;
using System.IO;
using System.Text;
using LabProject.Model;

public class FileLogger : StandardLogger
{
    public override string Name { get; }
    public string LogFile { get; }

    public FileLogger(string filename, string name)
    {
        this.Name = name;
        this.LogFile = filename;

        var directory = Path.GetDirectoryName(this.LogFile);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    protected override void Output(string text, LogLevel level)
    {
        var time = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff zzz] ");

        try
        {
            using var writer = new StreamWriter(File.Open(this.LogFile, FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8);

            writer.WriteLine(time + text);
            writer.Flush();
        }
        catch (Exception e)
        {
            if (LabProject.InstanceSet)
            {
                LabProject.Instance.InternalPrintOnlyLogger.Error($"Encountered an exception while logging to file {this.LogFile}: {e}");
            }
            else
            {
                throw;
            }
        }
    }
}