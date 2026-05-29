using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.title_categories — buckets for grouping
// titles in UI (achievement / pvp / event / etc., mig 0198).
public sealed record TitleCategoryDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    int SortOrder
);
