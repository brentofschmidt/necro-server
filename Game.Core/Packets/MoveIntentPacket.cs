using System.Numerics;
using System.Text.Json.Serialization;
using Game.Core.Network;

namespace Game.Core.Packets;

// Client → server every tick the player is trying to move. The client
// sends a DIRECTION, not a delta — the server picks the speed based on
// its own dt and the player's stats. This is what makes movement
// server-authoritative: the client can't lie about how fast it moved.
//
// Sequence is a monotonically increasing client-side counter used later
// for reconciliation: when the server corrects the client's predicted
// position, it echoes the highest sequence it has processed so the
// client knows which inputs to replay.
public sealed record MoveIntentPacket(
    Vector3 Direction,
    float Facing,
    uint Sequence
) : IPacket
{
    [JsonIgnore]
    public string Type => "move_intent";
}
