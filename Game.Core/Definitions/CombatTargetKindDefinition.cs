using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.combat_target_kinds — character / creature /
// world_container / door etc. Polymorphic catalog with target_schema/
// target_table for how a combat target id resolves.
public sealed record CombatTargetKindDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? TargetSchema,
    string? TargetTable,
    int SortOrder
);
