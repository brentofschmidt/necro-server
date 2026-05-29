using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.loot_pools — named drop pools. Pool entries
// live in loot_pool_entries (item, drop_chance, min_q, max_q).
// Previous loot_tables table coexists; pools are the newer
// composition model.
public sealed record LootPoolDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description
);
