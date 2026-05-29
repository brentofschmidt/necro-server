using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.affix_groups — exclusion groups; two affixes
// in the same group can't roll on the same item. Seeded from distinct
// affixes.affix_group values in mig 0201.
public sealed record AffixGroupDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    int SortOrder
);
