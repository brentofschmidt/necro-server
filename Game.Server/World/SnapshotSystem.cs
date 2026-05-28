using Game.Core.Packets;
using Game.Core.World;

namespace Game.Server.World;

// Builds and sends a WorldSnapshotPacket to every connected player each
// tick. Phase 4 is the simplest possible form: every visible entity,
// every tick, full snapshot (no diffing).
//
// Future passes will:
//   - Diff against last-acked snapshot per recipient
//   - Skip if nothing has changed in the recipient's view
//   - Pool the PlayerSnapshot[] arrays
//   - Split into player / mob / item sub-snapshots
internal static class SnapshotSystem
{
    public static void Tick(
        long tick,
        IReadOnlyDictionary<EntityId, PlayerSession> sessionsByEntity,
        IEnumerable<PlayerSession> sessions,
        SpatialGrid grid,
        WorldInstance world)
    {
        foreach (var s in sessions)
        {
            // Query the 3x3 cell block around the recipient. Includes the
            // recipient themselves — the client filters that out using its
            // WelcomePacket.YourEntityId.
            var visible = new List<PlayerSnapshot>();
            foreach (var id in grid.Query(s.Position))
            {
                if (sessionsByEntity.TryGetValue(id, out var other))
                {
                    visible.Add(new PlayerSnapshot(other.Id, other.DisplayName, other.Position, other.Facing));
                }
            }

            var pkt = new WorldSnapshotPacket(tick, s.LastProcessedSequence, visible.ToArray());
            world.SendTo(s.ConnectionId, pkt);
        }
    }
}
