namespace LabProject.Network;

using System;
using Mirror;

public class BotConnection : NetworkConnectionToClient
{
    private static int idGenerator = ushort.MaxValue;

    public override string address => "127.0.0.1";

    public override void Send(ArraySegment<byte> segment, int channelId = 0)
    {
    }

    protected override void SendToTransport(ArraySegment<byte> segment, int channelId = 0)
    {
    }

    public BotConnection() : base(idGenerator--)
    {

    }
}