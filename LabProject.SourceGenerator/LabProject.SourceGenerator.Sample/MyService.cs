namespace LabProject.SourceGenerator.Sample;

using System;
using LabApi.Events.Arguments.PlayerEvents;

[PlayerService]
public partial class MyService : ServiceBase
{
    protected override void OnPlayerJoined(PlayerJoinedEventArgs ev)
    {
        Console.WriteLine(ev.Player);
    }

    protected override void OnPlayerActivatedGenerator(PlayerActivatedGeneratorEventArgs ev)
    {
        base.OnPlayerActivatedGenerator(ev);
    }
}