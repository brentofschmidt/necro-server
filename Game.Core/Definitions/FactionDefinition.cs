using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.factions — reputation factions. Hostility
// pairs live in faction_hostility junction. starting_standing
// repointed to reputation_standings uuid in mig 0201.
public sealed record FactionDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    Guid? StartingStanding,
    string? IconPath,
    int SortOrder
);
