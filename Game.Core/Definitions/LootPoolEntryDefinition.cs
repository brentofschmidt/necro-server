using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.loot_pool_entries — one row per possible
// drop from a loot pool.
public sealed record LootPoolEntryDefinition(
    Guid Id,
    Guid LootPoolId,
    Guid ItemId,
    float DropChance,
    int MinQuantity,
    int MaxQuantity,
    int SortOrder
);
