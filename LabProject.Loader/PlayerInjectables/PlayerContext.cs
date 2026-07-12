namespace LabProject.Loader.PlayerInjectables;

using LabApi.Features.Wrappers;
using LabProject.DI;

public class PlayerContext : IPlayerContext
{
    public Player Player { get; internal set; } = null!;

    ReferenceHub IPlayerContext.GetPlayer()
    {
        return this.Player.ReferenceHub;
    }
}