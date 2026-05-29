using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.spells — magic equivalent of actions. Same
// shape as actions plus magic_school + splash columns; effects live
// in spells_effects junction.
public sealed record SpellDefinition(
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
    float Damage,
    Guid? DamageSchool,
    Guid? MagicSchool,
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
    float? SplashRadius,
    float? SplashDamageMultiplier,
    string? CastSoundKey,
    string? CastSoundLoop,
    float CastSoundDelay,
    float CastSoundVolume,
    string? ExecuteSoundKey,
    string? HitSoundKey,
    string? ImpactSoundKey
);
