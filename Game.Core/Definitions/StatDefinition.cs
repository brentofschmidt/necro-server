using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.stats — calculated stat metadata (armor,
// crit chance, dodge, parry, etc.). Combat pipeline reads these.
public sealed record StatDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    Guid? Category,
    string? Description,
    int SortOrder
);
