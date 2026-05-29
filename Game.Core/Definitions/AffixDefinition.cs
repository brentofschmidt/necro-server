using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.affixes — prefix/suffix rollable modifiers
// applied to item instances. `type` is FK to affix_types (mig 0207).
// applies_to_classes / applies_to_subclasses are uuid[] (mig 0205).
// Modifiers attached via affixes_modifiers junction.
public sealed record AffixDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    Guid Type,
    Guid? AffixGroup,
    Guid[] AppliesToClasses,
    Guid[] AppliesToSubclasses,
    int MinItemLevel,
    int Weight
);
