namespace LabProject.DI;

using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;

public interface ICommandManager
{
    public delegate bool ExecuteHandler(
        ArraySegment<string> arguments,
        ICommandSender sender,
        [UnscopedRef] out string response);

    void RegisterHandler(ExecuteHandler handler);

    void UnregisterHandler(ExecuteHandler handler);
}