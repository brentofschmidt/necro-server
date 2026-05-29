using System.Numerics;

namespace Game.Server.World;

// Applies each session's DesiredMove (set by MoveIntentHandler) to its
// position once per tick. Server-authoritative speed: clients send only
// a direction, the server picks the magnitude.
//
// DesiredMove is cleared after application. If a client wants to keep
// moving they must send a fresh MoveIntent every tick — a disconnected
// or lagged-out client coasts to a halt rather than dead-reckoning into
// the void.
internal static class MovementSystem
{
    // Per-second walking speed in meters. Tune later; eventually this
    // comes from stats + sprint state + encumbrance per GAME_DESIGN.md.
    public const float WalkSpeed = 5f;

    public static void Tick(float dt, IEnumerable<PlayerSession> sessions, SpatialGrid grid)
    {
        foreach (var s in sessions)
        {
            if (s.DesiredMove.LengthSquared() > 0f)
            {
                s.Position += s.DesiredMove * (WalkSpeed * dt);
                grid.Update(s.Id, s.Position);
            }
            s.DesiredMove = Vector3.Zero;
        }
    }
}
