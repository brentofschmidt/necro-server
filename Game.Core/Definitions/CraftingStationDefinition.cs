using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.crafting_stations — anvil, alchemy table,
// loom, etc. Where a recipe can be performed.
public sealed record CraftingStationDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    string? IconPath
);
