namespace LabProject.Vmf.Structures;

using LabProject.Vmf.Abstractions;

public class VersionInfoStructure(
    IDataProperty<int> editorVersion,
    IDataProperty<int> editorBuild,
    IDataProperty<int> mapVersion,
    IDataProperty<int> formatVersion,
    IDataProperty<bool> prefab) : IDataStructure
{
    public IDataProperty<int> EditorVersion { get; } = editorVersion;

    public IDataProperty<int> EditorBuild { get; } = editorBuild;

    public IDataProperty<int> MapVersion { get; } = mapVersion;

    public IDataProperty<int> FormatVersion { get; } = formatVersion;

    public IDataProperty<bool> Prefab { get; } = prefab;
}