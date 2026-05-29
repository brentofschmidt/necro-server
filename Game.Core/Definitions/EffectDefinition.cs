using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.effects — generic effect payload referenced
// by actions_effects / spells_effects / aura_effects / trigger_effects
// / items_consumable_effects (mig 0230). Wide-column shape replaces
// the original jsonb payload (mig 0232) — most columns are NULL per
// row; meaningful ones depend on effect type.
public sealed record EffectDefinition(
    Guid Id,
    Guid Type,
    decimal? Amount,
    decimal? Coefficient,
    Guid? School,
    Guid? Target,
    decimal? Duration,
    decimal? TickInterval,
    decimal? Radius,
    Guid? Stat,
    Guid? CalculationType,
    string? Description,
    Guid? Aura,
    Guid? Action,
    Guid? Creature,
    Guid? DestinationZone,
    decimal? X,
    decimal? Y,
    decimal? Z,
    decimal? Facing,
    Guid? Resource,
    decimal? Distance,
    decimal? Speed,
    int? Count,
    decimal? DirectionAngle
);
