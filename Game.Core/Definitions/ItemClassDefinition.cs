using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.item_classes — top-level WoW-style taxonomy
// bucket (weapon / armor / consumable / material / ...).
public sealed record ItemClassDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    int SortOrder
);
