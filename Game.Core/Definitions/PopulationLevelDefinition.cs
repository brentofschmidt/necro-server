using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.population_levels — low / medium / high /
// full vocabulary surfaced on realm cards (mig 0273).
public sealed record PopulationLevelDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    int MinPlayers,
    int? MaxPlayers,
    int SortOrder
);
