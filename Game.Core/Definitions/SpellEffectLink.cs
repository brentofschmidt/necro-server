using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.spells_effects — spells↔effects junction
// (mig 0230). Replaces the old spell_effects 1:N table.
public sealed record SpellEffectLink(
    Guid SpellId,
    Guid EffectId,
    int SortOrder
);
