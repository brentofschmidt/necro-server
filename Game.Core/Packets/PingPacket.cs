using System.Text.Json.Serialization;
using Game.Core.Network;

namespace Game.Core.Packets;

public sealed record PingPacket(long ClientTimeMs) : IPacket
{
    [JsonIgnore]
    public string Type => "ping";
}
