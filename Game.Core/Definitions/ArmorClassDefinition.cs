using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.armor_classes — cloth / leather / mail / plate /
// shield (proficiency gates).
public sealed record ArmorClassDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    int SortOrder
);
