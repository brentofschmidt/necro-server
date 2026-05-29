using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.spawn_types — creature / resource_node /
// world_container / npc (mig 0263). Polymorphic catalog using the
// target_schema/target_table pattern; spawn_points.target resolves
// through it.
public sealed record SpawnTypeDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? TargetSchema,
    string? TargetTable,
    int SortOrder
);
