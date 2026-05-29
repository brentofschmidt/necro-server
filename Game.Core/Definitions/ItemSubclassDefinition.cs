using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.item_subclasses — sword/axe/cloth/leather/etc.
// Each subclass belongs to one class (item_class FK).
public sealed record ItemSubclassDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    Guid? ItemClass,
    bool Stackable,
    Guid? InventorySlot,
    int SortOrder
);
