using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.mobs_modifiers — mobs↔modifiers junction
// (mig 0220). `mobs` is a template table, distinct from `creatures`.
public sealed record MobsModifierLink(
    Guid MobId,
    Guid ModifierId,
    int SortOrder
);
