using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.title_placements — prefix / suffix /
// other positional vocabulary for how the title renders relative
// to the character name (mig 0199).
public sealed record TitlePlacementDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    int SortOrder
);
