using System;
using System.Threading.Tasks;

namespace Game.Core.Network;

public delegate void PacketReceivedHandler(string connectionId, ReadOnlyMemory<byte> payload);
public delegate void ConnectionOpenedHandler(string connectionId, AuthContext auth);
public delegate void ConnectionClosedHandler(string connectionId);

public interface INetworkAdapter
{
    event PacketReceivedHandler? PacketReceived;
    event ConnectionOpenedHandler? ConnectionOpened;
    event ConnectionClosedHandler? ConnectionClosed;

    Task StartAsync(int port);
    Task StopAsync();
    Task SendAsync(string connectionId, ReadOnlyMemory<byte> payload);
}
