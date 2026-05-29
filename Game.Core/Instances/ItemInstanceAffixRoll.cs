using System;

namespace Game.Core.Instances;

// Mirrors necro_player.item_instance_affixes — per-instance rolled
// affix row (mig 0249). RolledValue is nullable; null = use the
// affix definition's flat value.
public sealed record ItemInstanceAffixRoll(
    Guid ItemInstanceId,
    int SortOrder,
    Guid AffixId,
    decimal? RolledValue
);
