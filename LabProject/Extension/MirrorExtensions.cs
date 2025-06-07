namespace LabProject.Extension;

using System;
using System.Reflection;
using Mirror;
using Object = UnityEngine.Object;

public static class MirrorExtensions
{
    private const string GetNextNetworkIdMethodName = "GetNextNetworkId";
    private const string AddToObservingMethodName = "AddToObserving";
    private const string RemoveFromObservingMethodName = "RemoveFromObserving";
    private const string RemoveOwnedObjectMethodName = "RemoveOwnedObject";
    private const string NetIdPropertyName = "netId";
    private const string ConnectionToClientPropertyName = "connectionToClient";

    private static readonly MethodInfo? GetNextNetworkIdMethod =
        typeof(NetworkIdentity).GetMethod(GetNextNetworkIdMethodName, BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? AddToObservingMethod =
        typeof(NetworkConnectionToClient).GetMethod(AddToObservingMethodName, BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly MethodInfo? RemoveFromObservingMethod =
        typeof(NetworkConnectionToClient).GetMethod(RemoveFromObservingMethodName, BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly MethodInfo? RemoveOwnedObjectMethod =
        typeof(NetworkConnectionToClient).GetMethod(RemoveOwnedObjectMethodName, BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly PropertyInfo? NetIdProperty =
        typeof(NetworkIdentity).GetProperty(NetIdPropertyName, BindingFlags.Public | BindingFlags.Instance);

    private static readonly PropertyInfo? ConnectionToClientProperty =
        typeof(NetworkIdentity).GetProperty(ConnectionToClientPropertyName, BindingFlags.Public | BindingFlags.Instance);

    public static void CreateObjectToServer(this NetworkIdentity identity, NetworkConnectionToClient connectionToClient)
    {
        identity.SetConnectionToClient(connectionToClient);

        var netId = GetNextNetworkId();
        identity.SetNetId(netId);
    }

    public static void DestroyObjectFromServer(this NetworkIdentity identity)
    {
        identity.connectionToClient.RemoveOwnedObject(identity);

        foreach (var connection in identity.observers.Values)
        {
            connection.DespawnObjectToConnection(identity);
        }

        identity.observers.Clear();
        identity.SetConnectionToClient(null);

        Object.Destroy(identity.gameObject);
    }

    public static void SpawnObjectToConnection(this NetworkConnectionToClient connectionToClient,
        NetworkIdentity identity)
    {
        connectionToClient.AddToObserving(identity);
        identity.observers.Add(connectionToClient.connectionId, connectionToClient);
    }

    public static void DespawnObjectToConnection(this NetworkConnectionToClient connectionToClient,
        NetworkIdentity identity)
    {
        connectionToClient.Send(new ObjectDestroyMessage { netId = identity.netId });
        connectionToClient.RemoveFromObserving(identity, true);
        identity.observers.Remove(connectionToClient.connectionId);
    }

    internal static uint GetNextNetworkId()
    {
        if (GetNextNetworkIdMethod == null)
        {
            throw new MissingMethodException("Unable to get GetNextNetworkId method from NetworkIdentity.");
        }

        return (uint)GetNextNetworkIdMethod.Invoke(null, null);
    }

    internal static void AddToObserving(this NetworkConnectionToClient observer, NetworkIdentity identity)
    {
        if (AddToObservingMethod == null)
        {
            throw new MissingMethodException("Unable to get AddToObserving method from NetworkConnectionToClient.");
        }

        AddToObservingMethod.Invoke(observer, BindingFlags.NonPublic, null!, [identity], null!);
    }

    internal static void RemoveFromObserving(this NetworkConnectionToClient observer, NetworkIdentity identity,
        bool isDestroyed)
    {
        if (RemoveFromObservingMethod == null)
        {
            throw new MissingMethodException(
                "Unable to get RemoveFromObserving method from NetworkConnectionToClient.");
        }

        RemoveFromObservingMethod.Invoke(observer, [identity, isDestroyed]);
    }

    internal static void RemoveOwnedObject(this NetworkConnectionToClient connectionToClient, NetworkIdentity identity)
    {
        if (RemoveOwnedObjectMethod == null)
        {
            throw new MissingMethodException("Unable to get RemoveOwnedObject method from NetworkConnectionToClient.");
        }

        RemoveOwnedObjectMethod.Invoke(connectionToClient, [identity]);
    }

    internal static void SetNetId(this NetworkIdentity identity, uint netId)
    {
        if (NetIdProperty == null)
        {
            throw new MissingMethodException("Unable to get netId property from NetworkIdentity.");
        }

        NetIdProperty.SetValue(identity, netId);
    }

    internal static void SetConnectionToClient(this NetworkIdentity identity,
        NetworkConnectionToClient? connectionToClient)
    {
        if (ConnectionToClientProperty == null)
        {
            throw new MissingMethodException("Unable to get connectionToClient property from NetworkIdentity.");
        }

        ConnectionToClientProperty.SetValue(identity, connectionToClient);
    }
}