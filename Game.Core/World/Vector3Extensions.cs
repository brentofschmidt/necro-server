using System;
using System.Numerics;

namespace Game.Core.World;

// Game.Core uses System.Numerics.Vector3 for shared types (see
// GAME_CORE_SHARING.md). These extensions add the two operations
// we actually need that System.Numerics doesn't ship safely:
//
//   * NormalizedSafe — Vector3.Normalize() returns NaN-tainted
//     vectors for near-zero inputs, which silently corrupts every
//     downstream calculation. This returns (Zero, 0) instead so
//     the caller can detect a zero-length intent.
//
//   * IsFinite — guard at network boundaries. A client that
//     sends Direction=(NaN, 0, 0) (deliberately or by bug) would
//     otherwise propagate NaN into the simulation. Reject upfront.
public static class Vector3Extensions
{
    private const float MinLengthSquared = 1e-8f;

    public static (Vector3 Direction, float Length) NormalizedSafe(this Vector3 v)
    {
        var lenSq = v.LengthSquared();
        if (lenSq < MinLengthSquared) return (Vector3.Zero, 0f);
        var len = MathF.Sqrt(lenSq);
        return (v / len, len);
    }

    public static bool IsFinite(this Vector3 v)
        => float.IsFinite(v.X) && float.IsFinite(v.Y) && float.IsFinite(v.Z);
}
