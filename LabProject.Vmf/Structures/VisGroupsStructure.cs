namespace LabProject.Vmf.Structures;

using System.Collections.Generic;
using LabProject.Vmf.Abstractions;

public class VisGroupsStructure(IReadOnlyCollection<IDataStructure> visGroups)
{
    public IReadOnlyCollection<IDataStructure> VisGroups { get; } = visGroups;
}