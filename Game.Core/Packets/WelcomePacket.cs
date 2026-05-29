using System;
using System.Numerics;
using System.Text.Json.Serialization;
using Game.Core.Network;
using Game.Core.World;

namespace Game.Core.Packets;

// Sent server → client once, immediately after a connection authenticates
// (real or stub). Tells the client which entity in future snapshots IS
// them, plus realm info for display. Replaced by a richer
// InitialStateSnapshot in Phase 3 proper.
public sealed record WelcomePacket(
    EntityId YourEntityId,
    string YourName,
    Guid RealmId,
    string RealmName,
    Vector3 SpawnPosition
) : IPacket
{
    [JsonIgnore]
    public string Type => "welcome";
}
