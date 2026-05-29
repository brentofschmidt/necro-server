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

    // Server-initiated disconnect. Called when the world rejects a
    // connection after it has already authenticated at the transport
    // layer (e.g. unknown subject, duplicate session, kick). No-op if
    // the connection is already gone — the ConnectionClosed event
    // still fires from the receive loop's finally block.
    Task CloseAsync(string connectionId, string reason);
}
