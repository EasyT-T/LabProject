namespace LabProject.Command;

using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using LabApi.Features.Wrappers;

[CommandHandler(typeof(ClientCommandHandler))]
public class TpsCommand : ICommand
{
    public string Command => "tps";
    public string[]? Aliases => null;
    public string Description => "Get current tps.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        response = $"TPS: {Server.Tps} / {Server.MaxTps}";
        return true;
    }
}