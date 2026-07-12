namespace LabProject.Loader.Injectables;

using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using LabProject.Attributes;
using LabProject.DI;

[Injectable]
public class CommandManager : IService, ICommandManager
{
    private class CustomCommand(string commandName, string[]? aliases, string description, ICommandManager.ExecuteHandler executeHandler) : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
        {
            return executeHandler(arguments, sender, out response);
        }

        public string Command { get; } = commandName;
        public string[]? Aliases { get; } = aliases;
        public string Description { get; } = description;
    }

    public void RegisterHandler(ICommandManager.ExecuteHandler handler)
    {
        throw new NotImplementedException();
    }

    public void UnregisterHandler(ICommandManager.ExecuteHandler handler)
    {
        throw new NotImplementedException();
    }
}