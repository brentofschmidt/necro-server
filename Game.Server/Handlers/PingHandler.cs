using System.Text.Json;
using Game.Core.Network;
using Game.Core.Packets;
using Microsoft.Extensions.Logging;

namespace Game.Server.Handlers;

internal static class PingHandler
{
    public static void Handle(PingPacket packet, string connectionId, INetworkAdapter network, ILogger logger)
    {
        var pong = new PongPacket(packet.ClientTimeMs, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        var envelope = PacketEnvelope.Of(pong);
        var bytes = JsonSerializer.SerializeToUtf8Bytes(envelope);
        // Fire-and-forget — the send is async but the tick must not block on it.
        _ = network.SendAsync(connectionId, bytes);
        logger.LogDebug("pong → {ConnId} (client {Ct} server {St})", connectionId, packet.ClientTimeMs, pong.ServerTimeMs);
    }
}
