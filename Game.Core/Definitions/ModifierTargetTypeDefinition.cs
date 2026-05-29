using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.modifier_target_types — Stat / Resource /
// Ability. Renamed from effect_types in mig 0231 (the old name was
// reused for the deal_damage/heal effect kind).
public sealed record ModifierTargetTypeDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    int SortOrder
);
