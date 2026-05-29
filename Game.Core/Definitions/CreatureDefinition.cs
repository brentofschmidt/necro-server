using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.creatures — catalog of fight-able entities.
// `type` FK to creature_types added in mig 0235 / family dropped in
// 0236. Modifiers (per-mob bonuses) live in mobs_modifiers junction
// — note `mobs` is a separate per-instance/template table from
// `creatures` in the current schema.
public sealed record CreatureDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string Description,
    Guid? Type,
    Guid? Size,
    int Level,
    int MaxHealth,
    int Armor,
    int MagicResist,
    int AttackDamageMin,
    int AttackDamageMax,
    Guid? AttackDamageType,
    float AttackSpeed,
    float MovementSpeed,
    bool IsAggressive,
    float AggroRange,
    int BaseXp,
    Guid? LootTableId,
    string? IconPath,
    string DisplayColor
);
