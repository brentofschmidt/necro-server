using System.Text.Json.Serialization;
using Game.Core.Network;
using Game.Core.World;

namespace Game.Core.Packets;

// Server → client every tick: the entities this player can see right now.
// Phase 4 is a "full" snapshot — every visible entity, every tick. Phase
// 5+ swaps in delta-since-last-acked snapshots once we have the bandwidth
// math to care.
//
// LastProcessedSequence is the highest MoveIntent.Sequence the server has
// applied for the recipient. The client uses this to reconcile its
// predicted position against the authoritative one.
public sealed record WorldSnapshotPacket(
    long Tick,
    uint LastProcessedSequence,
    PlayerSnapshot[] Players
) : IPacket
{
    [JsonIgnore]
    public string Type => "world_snapshot";
}
