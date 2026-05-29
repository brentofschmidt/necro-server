using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.abilities — the six D&D ability scores
// (STR/DEX/CON/INT/WIS/CHA). Renamed from the old "active abilities"
// table in migration 0019; what used to live here is now in actions.
public sealed record AbilityDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    Guid? Category,
    string? Description,
    int SortOrder
);
