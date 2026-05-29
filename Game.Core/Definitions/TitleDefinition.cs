using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.titles — earnable character titles (mig 0198
// added title_categories, 0199 added title_placements).
public sealed record TitleDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string Description,
    Guid? Category,
    Guid Placement,
    int SortOrder
);
