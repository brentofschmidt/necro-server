using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.races — playable races. starting_abilities
// dropped in mig 0258; race modifiers live in races_modifiers
// junction.
public sealed record RaceDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    string? IconPath,
    int SortOrder
);
