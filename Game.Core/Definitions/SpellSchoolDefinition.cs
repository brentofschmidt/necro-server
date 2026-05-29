using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.spell_schools — arcane / divine / nature / etc.
public sealed record SpellSchoolDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    int SortOrder
);
