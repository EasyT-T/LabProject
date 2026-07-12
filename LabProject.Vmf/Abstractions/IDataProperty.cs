namespace LabProject.Vmf.Abstractions;

public interface IDataProperty<out T>
{
    string Key { get; }

    T Value { get; }
}