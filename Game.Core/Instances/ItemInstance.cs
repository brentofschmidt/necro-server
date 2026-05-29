using System;
using System.Collections.Generic;

namespace Game.Core.Instances;

// Mirrors necro_player.item_instances — per-unique-copy runtime
// row for non-stackable items so sockets / enchants / affix rolls /
// durability travel with the gear piece as it moves through the
// world. NULL owner means the instance currently lives in the world
// (floor / corpse pile). Per-instance state was normalized into
// junction tables in mig 0249 — Affixes / Enchants / Sockets are
// loaded as children alongside the row.
public sealed record ItemInstance(
    Guid Id,
    Guid ItemId,
    Guid? OwnerUserId,
    Guid? OwnerCharacterId,
    Guid? RealmId,
    DateTime CreatedAt,
    int? DurabilityCurrent,
    int? DurabilityMax,
    string? CustomName,
    IReadOnlyList<ItemInstanceAffixRoll> Affixes,
    IReadOnlyList<ItemInstanceEnchant> Enchants,
    IReadOnlyList<ItemInstanceSocket> Sockets
);
