namespace LabProject.Model;

using System;
using System.Diagnostics.CodeAnalysis;
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
    private const string DedicatedIdConstName = "DedicatedId";
    private const string TimeoutTimerFieldName = "_timeoutTimer";

    private static readonly FieldInfo? DedicatedIdConst =
        typeof(PlayerAuthenticationManager).GetField(DedicatedIdConstName,
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly FieldInfo? TimeoutTimerField =
        typeof(PlayerAuthenticationManager).GetField(TimeoutTimerFieldName, BindingFlags.NonPublic | BindingFlags.Instance);

    [MemberNotNullWhen(true, nameof(Observer))]
    public bool Spawned { get; private set; }
    public bool Destroyed { get; private set; }

    public Player? Observer { get; private set; }
    public GameObject GameObject { get; }
    public ReferenceHub ReferenceHub { get; }

    public Bot(string? nickname = null)
    {
        var botObject = Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.playerPrefab);
        var botHub = botObject.GetComponent<ReferenceHub>();
        botHub.nicknameSync.MyNick = nickname;
        Object.Destroy(botObject.GetComponent<VersionCheck>());

        ReferenceHub.AllHubs.Remove(botHub);

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

        botHub.networkIdentity.CreateObjectToServer(new BotConnection());

        if (DedicatedIdConst == null)
        {
            this.GetInternalLogger()
                .Warn(
                    $"Unable to get dedicated id constant from authentication manager while creating a bot (Nickname: {nickname}). Bot may cause authentication exceptions. (Constant name: '{DedicatedIdConstName}')");
        }

        botHub.authManager.NetworkSyncedUserId = (string)(DedicatedIdConst?.GetValue(null) ?? string.Empty);

        this.GetInternalLogger().Debug(botHub.authManager.InstanceMode.ToString());

        this.GameObject = botObject;
        this.ReferenceHub = botHub;
    }

    ~Bot()
    {
        this.Destroy();

        this.GetInternalLogger().Debug("Bot " + this.ReferenceHub.PlayerId + " has been destroyed automatically.");
    }

    public void Spawn(Player observer)
    {
        if (this.Spawned || this.Destroyed)
        {
            return;
        }

        observer.ReferenceHub.connectionToClient.SpawnObjectToConnection(this.ReferenceHub.netIdentity);

        this.Observer = observer;
        this.Spawned = true;
    }

    public void Despawn()
    {
        if (!this.Spawned || this.Destroyed)
        {
            return;
        }

        this.Observer.ReferenceHub.connectionToClient.DespawnObjectToConnection(this.ReferenceHub.netIdentity);

        this.Observer = null;
        this.Spawned = false;
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