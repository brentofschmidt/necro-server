using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.zones — world zones / regions. pvp_rules
// and biomes added in mig 0217.
public sealed record ZoneDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string Description,
    Guid Biome,
    Guid PvpRule,
    int RecommendedLevelMin,
    int RecommendedLevelMax
);
