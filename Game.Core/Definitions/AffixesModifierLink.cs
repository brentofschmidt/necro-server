using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.affixes_modifiers — affixes↔modifiers
// junction (mig 0220).
public sealed record AffixesModifierLink(
    Guid AffixId,
    Guid ModifierId,
    int SortOrder
);
