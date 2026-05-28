using System.Text.Json.Serialization;
using Game.Core.Network;

namespace Game.Core.Packets;

public sealed record PongPacket(long ClientTimeMs, long ServerTimeMs) : IPacket
{
    [JsonIgnore]
    public string Type => "pong";
}
