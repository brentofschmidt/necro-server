using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.actions — physical things you do with weapons
// (Slash, Power Strike, Block). Spells live in SpellDefinition.
// No intrinsic damage: damage comes from the equipped weapon at runtime
// (migration 0033). Effects are linked via actions_effects.
public sealed record ActionDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    string? IconPath,
    Guid? Type,
    Guid? Targeting,
    Guid? ResourceType,
    float ResourceCost,
    float Cooldown,
    float CastTime,
    float GlobalCooldown,
    float Range,
    bool RequiresTarget,
    bool IsHeal,
    bool IsToggle,
    bool CancelCastOnMove,
    string? AnimTrigger,
    string? AnimChannelBool,
    float AnimationDelay,
    float AnimTriggerOffset,
    float HitDelay,
    string? CastEffectKey,
    string? HitEffectKey,
    string? ImpactEffectKey,
    string? ProjectileKey,
    float? ProjectileSpeed,
    string? CastSoundKey,
    string? CastSoundLoop,
    float CastSoundDelay,
    float CastSoundVolume,
    string? ExecuteSoundKey,
    string? HitSoundKey,
    string? ImpactSoundKey
);
