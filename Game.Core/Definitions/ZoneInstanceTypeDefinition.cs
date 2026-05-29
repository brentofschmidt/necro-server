using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.zone_instance_types — open world / dungeon /
// raid / arena vocabulary (mig 0216).
public sealed record ZoneInstanceTypeDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    int SortOrder
);
