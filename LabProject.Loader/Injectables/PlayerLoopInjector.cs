namespace LabProject.Loader.Injectables;

using Cysharp.Threading.Tasks;
using LabProject.Attributes;
using LabProject.DI;
using Microsoft.Extensions.Logging;
using UnityEngine.LowLevel;

[Injectable(priority: int.MinValue)]
public class PlayerLoopInjector(ILogger<PlayerLoopInjector> logger) : IService, ILoad
{
    private static bool loaded;

    void ILoad.Load()
    {
        if (loaded)
        {
            return;
        }

        var playerLoop = PlayerLoop.GetCurrentPlayerLoop();

        PlayerLoopHelper.Initialize(ref playerLoop);

        loaded = true;

        logger.LogInformation("UniTask PlayerLoop initialized.");
    }
}