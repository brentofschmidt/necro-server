using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.aura_effects — auras↔effects junction
// (mig 0265). Per-tick payload moved here from auras.periodic_*
// columns.
public sealed record AuraEffectLink(
    Guid Id,
    Guid AuraId,
    Guid EffectId,
    int SortOrder
);
