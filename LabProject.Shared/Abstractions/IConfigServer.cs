namespace LabProject.Abstractions;

using System.Diagnostics.CodeAnalysis;

public interface IConfigServer
{
    T RegisterConfig<T>(string name) where T : class, new();

    T GetConfig<T>(string name) where T : class, new();

    bool TryGetConfig<T>(string name, [MaybeNullWhen(false)] out T config) where T : class, new();
}