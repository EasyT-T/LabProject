namespace LabProject.Model;

using System;
using System.Reflection;
using CentralAuth;
using global::LabProject.Extension;
using global::LabProject.Logging;
using global::LabProject.Network;
using LabApi.Features.Wrappers;
using Mirror.LiteNetLib4Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

public class Bot : ILoggerProvider, IDisposable
{
    private const string TimeoutTimerFieldName = "_timeoutTimer";

    private static readonly FieldInfo? TimeoutTimerField =
        typeof(PlayerAuthenticationManager).GetField(TimeoutTimerFieldName, BindingFlags.NonPublic | BindingFlags.Instance);

    public bool Spawned { get; private set; }
    public bool Destroyed { get; private set; }

    public Player Observer { get; }
    public GameObject GameObject { get; }
    public ReferenceHub ReferenceHub { get; }

    public Bot(Player observer, string nickname)
    {
        var botObject = Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.playerPrefab);
        var botHub = botObject.GetComponent<ReferenceHub>();
        botHub.nicknameSync.MyNick = nickname;

        ReferenceHub.AllHubs.Remove(botHub);
        Object.Destroy(botHub.queryProcessor);

        if (TimeoutTimerField == null)
        {
            this.GetInternalLogger()
                .Warn(
                    $"Unable to get timout timer field from authentication manager while creating a bot (Nickname: {nickname}). Bot may cause authentication exceptions. (Field name: '{TimeoutTimerFieldName}')");
        }
        else
        {
            TimeoutTimerField.SetValue(botHub.authManager, -1.0f);
        }

        this.Observer = observer;
        this.GameObject = botObject;
        this.ReferenceHub = botHub;
    }

    ~Bot()
    {
        this.Destroy();
    }

    public void Spawn()
    {
        if (this.Spawned || this.Destroyed)
        {
            return;
        }

        this.Observer.ReferenceHub.connectionToClient.SpawnObjectToConnection(this.ReferenceHub.netIdentity,
            new BotConnection());

        this.Spawned = true;
    }

    public void Dispose()
    {
        if (this.Destroyed)
        {
            return;
        }

        this.Destroy();

        GC.SuppressFinalize(this);
    }

    private void Destroy()
    {
        this.Destroyed = true;

        this.ReferenceHub.netIdentity.DestroyObjectFromServer();
    }
}