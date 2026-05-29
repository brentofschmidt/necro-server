using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.items_modifiers — items↔modifiers junction
// (mig 0220). One row per modifier on an item; ordered by sort_order.
public sealed record ItemsModifierLink(
    Guid ItemId,
    Guid ModifierId,
    int SortOrder
);
