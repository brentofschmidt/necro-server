using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.modifiers — the master modifier row
// referenced by items_modifiers / affixes_modifiers / etc. (mig 0220).
// `affects` is polymorphic — interpret via target_type (Stat/Resource/
// Ability). Renamed columns: type→target_type, modifier_type→
// calculation_type (mig 0231).
public sealed record ModifierDefinition(
    Guid Id,
    Guid TargetType,
    Guid Affects,
    decimal Value,
    Guid CalculationType,
    string? Description
);
