namespace LabProject.Command;

using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using LabApi.Features.Wrappers;
using LabProject.Model;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
// ReSharper disable once UnusedType.Global
public class BotCommand : ICommand
{
    public string Command => "bot";
    public string[]? Aliases => null;
    public string Description => "Advanced bot commands.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        var nickname = arguments.At(0);
        var player = Player.Get(sender);

        if (player == null)
        {
            response = "Please specify the command after authenticated.";
            return false;
        }

        var bot = new Bot(player, nickname);
        bot.Spawn();

        response = "Done!";
        return true;
    }
}