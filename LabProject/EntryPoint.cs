namespace LabProject;

using System;
using System.IO;
using LabApi.Loader.Features.Plugins;
using LabProject.Model;

internal class EntryPoint : Plugin
{
    public override string Name => "LabProject";
    public override string Description => "A reimplementation of a plugin I once wrote.";
    public override string Author => "EasyT_T";
    public override Version Version { get; } = new Version(0, 1, 0);
    public override Version RequiredApiVersion { get; } = new Version(0, 7, 0);

    public override void Enable()
    {
        LabProject.Create(this.Name, Path.Combine(Path.GetDirectoryName(this.FilePath) ?? string.Empty, "LabProject", "Logs"));
    }

    public override void Disable()
    {
        LabProject.Destroy();
    }
}