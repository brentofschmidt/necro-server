using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.item_bag_specs — 1:1 extension of items for
// bag-only properties (mig 0275). Most items have no row here; only
// items that ARE bags do. Joining is cheap and keeps items skinny.
public sealed record ItemBagSpecsDefinition(
    Guid ItemId,
    int SlotCount,
    int? MaxWeight,
    short? Width,
    short? Height,
    bool IsSecure,
    Guid? AcceptsItemSubclass
);
