using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.effect_types — the effect kind catalog
// (deal_damage / heal / apply_aura / teleport / etc.). Renamed
// from effect_kinds in mig 0231.
public sealed record EffectTypeDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    int SortOrder
);
