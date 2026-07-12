namespace LabProject.Loader.Injectables;

using LabApi.Loader.Features.Plugins;
using LabProject.Attributes;
using LabProject.DI;

[Injectable]
public class PathProvider : IService
{
    public string PluginPath { get; }

    public string ConfigPath { get; }

    public PathProvider(Plugin plugin)
    {
        this.PluginPath = Path.GetDirectoryName(plugin.FilePath) ?? string.Empty;

        this.ConfigPath = Path.Combine(this.PluginPath, "lab-config");

        if (!Directory.Exists(this.ConfigPath))
        {
            Directory.CreateDirectory(this.ConfigPath);
        }
    }
}