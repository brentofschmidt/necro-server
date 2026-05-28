using Game.Core.World;

namespace Game.Server.World;

// Per-realm runtime state for one logged-in player. Lives entirely on the
// tick thread — never mutated from network threads. Snapshots derived from
// this each tick are what go on the wire.
public sealed class PlayerSession
{
    public EntityId Id { get; }
    public string ConnectionId { get; set; }
    public string Subject { get; }
    public string DisplayName { get; }

    public Vec3 Position { get; set; }
    public float Facing { get; set; }

    // Latest direction the client wants to move (normalized). Cleared each
    // tick after MovementSystem applies it. NOT a position delta; the
    // server picks the speed.
    public Vec3 DesiredMove { get; set; } = Vec3.Zero;

    // Highest MoveIntent.Sequence we've applied for this player. Echoed in
    // outgoing snapshots so the client knows which inputs have been
    // reconciled.
    public uint LastProcessedSequence { get; set; }

    public PlayerSession(EntityId id, string connectionId, string subject, string displayName, Vec3 spawn)
    {
        Id = id;
        ConnectionId = connectionId;
        Subject = subject;
        DisplayName = displayName;
        Position = spawn;
    }
}
