namespace LabProject.Loader.PlayerInjectables;

using LabApi.Events.Handlers;
using LabProject.DI;
using LabProject.SourceGenerator;

[PlayerServiceBase(typeof(PlayerEvents))]
public abstract partial class PlayerService;