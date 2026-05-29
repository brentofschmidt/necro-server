using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.modifier_calculation_types — Flat / Percent /
// Ratio / PerLevel (mig 0281 added per-level scaling). Renamed from
// modifier_types in mig 0231.
public sealed record ModifierCalculationTypeDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    int SortOrder
);
