namespace LabProject.Loader.Injectables;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using CommunityToolkit.Diagnostics;
using LabProject.Abstractions;
using LabProject.Attributes;using LabProject.DI;

[Injectable]
public class LocalConfigServer(PathProvider pathProvider) : IService, IConfigServer
{
    public static string ConfigExtension = "json";

    private readonly Dictionary<string, object> _configs = new Dictionary<string, object>();

    private readonly JsonSerializerOptions _options = new JsonSerializerOptions(JsonSerializerDefaults.General)
    {
        WriteIndented = true,
    };

    public T RegisterConfig<T>(string name) where T : class, new()
    {
        if (!this.TryGetConfigFromFile<T>(name, out var config))
        {
            config = this.CreateDefaultConfig<T>(name);
        }

        this._configs.Add(name, config!);

        return config;
    }

    public T GetConfig<T>(string name) where T : class, new()
    {
        this.TryGetConfig<T>(name, out var config);

        Guard.IsNotNull(config);

        return config;
    }

    public bool TryGetConfig<T>(string name, [MaybeNullWhen(false)] out T config)
        where T : class, new()
    {
        config = null;

        if (this._configs.TryGetValue(name, out var configObject))
        {
            config = configObject as T;
        }
        else
        {
            config = this.RegisterConfig<T>(name);
        }

        return config != null;
    }

    private bool TryGetConfigFromFile<T>(string name, [MaybeNullWhen(false)] out T config) where T : class
    {
        var path = Path.Combine(pathProvider.ConfigPath, name + ".json");

        if (!File.Exists(path))
        {
            config = null;
            return false;
        }

        using var stream = File.OpenRead(path);

        var result = JsonSerializer.Deserialize<T>(stream);

        config = result;

        return result != null;
    }

    private T CreateDefaultConfig<T>(string name)
        where T : class, new()
    {
        var path = Path.Combine(pathProvider.ConfigPath, name + ".json");

        var config = new T();

        using var stream = File.OpenWrite(path);

        JsonSerializer.Serialize(stream, config, this._options);

        return config;
    }
}