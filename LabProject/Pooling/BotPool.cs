namespace LabProject.Pooling;

using System;
using System.Collections.Concurrent;
using LabApi.Features.Wrappers;
using LabProject.Model;
using PlayerRoles;

public class BotPool : IDisposable
{
    public const int MaxSize = 32;

    public static BotPool Shared { get; } = new BotPool();

    private readonly ConcurrentBag<Bot> pool = [];

    public Bot Rent(Player observer, string? nickname = null)
    {
        if (this.pool.TryTake(out var bot))
        {
            bot.Spawn(observer);
            bot.ReferenceHub.nicknameSync.MyNick = nickname;
            return bot;
        }

        bot = new Bot(nickname);
        bot.Spawn(observer);

        return bot;
    }

    public void Return(Bot bot)
    {
        if (bot.Destroyed)
        {
            return;
        }

        if (this.pool.Count >= MaxSize)
        {
            bot.Dispose();
            return;
        }

        bot.Despawn();
        bot.ReferenceHub.nicknameSync.MyNick = null;
        bot.ReferenceHub.roleManager.ServerSetRole(RoleTypeId.None, RoleChangeReason.Destroyed);
        bot.ReferenceHub.serverRoles.SetText(null);
        bot.ReferenceHub.serverRoles.SetColor(null);

        this.pool.Add(bot);
    }

    public void Dispose()
    {
        foreach (var bot in this.pool)
        {
            bot.Dispose();
        }
    }
}