using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.resources — HP / mana / stamina catalog.
public sealed record ResourceDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string Description,
    string DisplayColor,
    int SortOrder
);
