namespace Game.Core.World;

// 3D position / direction in world space. Deliberately NOT named Vector3 —
// Unity has its own UnityEngine.Vector3 with different semantics (mutable
// struct, lots of operator overloads). Keeping this as a plain immutable
// record struct means the wire format stays small and unambiguous and
// Unity code stays explicit about when it's converting between the two.
public readonly record struct Vec3(float X, float Y, float Z)
{
    public static Vec3 Zero => default;

    public Vec3 Add(Vec3 other) => new(X + other.X, Y + other.Y, Z + other.Z);

    public Vec3 Scale(float s) => new(X * s, Y * s, Z * s);

    public float LengthSquared() => X * X + Y * Y + Z * Z;

    // Returns (Vec3.Zero, 0) if input has near-zero length. Caller can
    // detect a zero-length intent by checking the returned length.
    public (Vec3 Direction, float Length) Normalized()
    {
        var lenSq = LengthSquared();
        if (lenSq < 1e-8f) return (Zero, 0f);
        var len = (float)System.Math.Sqrt(lenSq);
        return (new Vec3(X / len, Y / len, Z / len), len);
    }

    public override string ToString() => $"({X:F2},{Y:F2},{Z:F2})";
}
