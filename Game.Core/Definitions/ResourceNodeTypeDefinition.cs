using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.resource_node_types — ore_vein / tree /
// herb_patch / fishing_hole vocabulary (mig 0214).
public sealed record ResourceNodeTypeDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    int SortOrder
);
