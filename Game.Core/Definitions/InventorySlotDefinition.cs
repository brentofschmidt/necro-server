using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.inventory_slots — equipment slot vocabulary
// (Head / Chest / MainHand / OffHand / TwoHand / Trinket1 / etc.).
public sealed record InventorySlotDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    Guid? BodyRegion,
    int SortOrder
);
