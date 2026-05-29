using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.actions_effects — actions↔effects junction
// (mig 0230). Replaces the old action_effects 1:N table.
public sealed record ActionEffectLink(
    Guid ActionId,
    Guid EffectId,
    int SortOrder
);
