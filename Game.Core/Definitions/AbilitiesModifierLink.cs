using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.abilities_modifiers — abilities↔modifiers
// junction (mig 0220, renamed from ability_derived_effects).
public sealed record AbilitiesModifierLink(
    Guid AbilityId,
    Guid ModifierId,
    int SortOrder
);
