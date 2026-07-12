namespace LabProject.Vmf.Properties;

using LabProject.Vmf.Abstractions;

public class IntProperty(string key, int value) : IDataProperty<int>
{
    public string Key { get; } = key;

    public int Value { get; } = value;
}