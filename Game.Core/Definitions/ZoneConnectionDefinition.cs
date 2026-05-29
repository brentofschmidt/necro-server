using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.zone_connections — directed edges between
// adjacent zones (portal / road / sea-route / etc.). connection_type
// normalized to uuid FK in mig 0233.
public sealed record ZoneConnectionDefinition(
    Guid Id,
    string Slug,
    Guid FromZone,
    Guid ToZone,
    Guid? ConnectionType,
    string? Description
);
