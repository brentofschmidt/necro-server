using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.races_modifiers — races↔modifiers junction
// (mig 0220). Encodes per-race stat/ability bonuses.
public sealed record RacesModifierLink(
    Guid RaceId,
    Guid ModifierId,
    int SortOrder
);
