namespace LabProject.Loader.Injectables;

using Cysharp.Threading.Tasks;
using LabProject.Abstractions;
using LabProject.Attributes;
using LabProject.DI;
using Microsoft.Extensions.Logging;

[Injectable(priority: int.MinValue)]
public class LabExceptionHandler(ILogger<LabExceptionHandler> logger) : IService, ILoad, IUnload, IExceptionHandler
{
    public void HandleException(Exception exception)
    {
        logger.LogError(exception, "Exception captured");
    }

    public void HandleException(Exception exception, string message)
    {
        logger.LogError(exception, "Exception captured: {Message}", message);
    }

    void ILoad.Load()
    {
        UniTaskScheduler.UnobservedTaskException += this.HandleException;
    }

    void IUnload.Unload()
    {
        UniTaskScheduler.UnobservedTaskException -= this.HandleException;
    }
}