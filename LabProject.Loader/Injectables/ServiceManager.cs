namespace LabProject.Loader.Injectables;

using LabProject.Abstractions;
using LabProject.Attributes;
using LabProject.DI;
using LabProject.Loader.Extensions;

[Injectable]
public class ServiceManager(
    IEnumerable<ILoad> loads,
    IEnumerable<IUnload> unloads,
    IExceptionHandler exceptionHandler)
{
    public void Load()
    {
        foreach (var load in loads)
        {
            load.LoadSafely(exceptionHandler);
        }
    }

    public void Unload()
    {
        foreach (var unload in unloads)
        {
            unload.UnloadSafely(exceptionHandler);
        }
    }
}