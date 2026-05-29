using System.Numerics;

namespace Game.Core.World;

// One player's visible-to-others state for a given tick. Sent inside
// WorldSnapshotPacket. Will grow over time (HP, mana, current cast,
// equipped weapon for visuals) — for Phase 4 just enough to render a
// moving dot with a name above it.
public sealed record PlayerSnapshot(
    EntityId Id,
    string Name,
    Vector3 Position,
    float Facing
);
