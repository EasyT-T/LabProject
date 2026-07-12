namespace LabProject.Vmf.Structures;

using LabProject.Vmf.Abstractions;

public class MapStructure(
    IDataStructure versionInfo,
    IDataStructure visGroups,
    IDataStructure viewSettings,
    IDataStructure world,
    IDataStructure entity,
    IDataStructure hidden,
    IDataStructure cameras,
    IDataStructure cordon)
    : IDataStructure
{
    public IDataStructure VersionInfo { get; } = versionInfo;

    public IDataStructure VisGroups { get; } = visGroups;

    public IDataStructure ViewSettings { get; } = viewSettings;

    public IDataStructure World { get; } = world;

    public IDataStructure Entity { get; } = entity;

    public IDataStructure Hidden { get; } = hidden;

    public IDataStructure Cameras { get; } = cameras;

    public IDataStructure Cordon { get; } = cordon;
}