using System;

namespace Game.Core.Instances;

// Mirrors necro_player.active_auras — per-character currently
// applied auras. (character_id, instance_id) is the PK; multiple
// instances of the same aura_id can stack via distinct instance
// strings. RemainingTime in seconds; 0 with passive duration=0
// means "permanent" (never expires).
public sealed record AuraInstance(
    Guid Id,
    Guid CharacterId,
    Guid AuraId,
    string Slug,
    float RemainingTime,
    int Stacks,
    float TickTimer,
    DateTime AppliedAtUtc,
    string CasterName
);
