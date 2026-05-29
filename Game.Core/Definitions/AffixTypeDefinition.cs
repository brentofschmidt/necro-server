using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.affix_types — 'prefix' / 'suffix' catalog
// (mig 0206). Replaces the old text discriminator on affixes.
public sealed record AffixTypeDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    int SortOrder
);
