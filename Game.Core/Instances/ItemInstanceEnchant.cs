using System;

namespace Game.Core.Instances;

// Mirrors necro_player.item_instance_enchants — per-instance
// applied enchant (mig 0249).
public sealed record ItemInstanceEnchant(
    Guid ItemInstanceId,
    int SortOrder,
    Guid EnchantId
);
