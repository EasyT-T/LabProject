namespace LabProject;

using CommunityToolkit.Diagnostics;
using LabApi.Events.Arguments.PlayerEvents;
using LabProject.Loader.PlayerInjectables;
using LabProject.SourceGenerator;
using Microsoft.Extensions.Logging;

[PlayerService]
public partial class MyPlayerService(PlayerContext context, ILogger<MyPlayerService> logger) : PlayerService
{
    protected override void OnPlayerJoined(PlayerJoinedEventArgs ev)
    {
        Guard.IsReferenceEqualTo(ev.Player, context.Player);

        logger.LogInformation("Player: {Player}", context.Player);
    }
}