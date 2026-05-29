using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.objective_types — kill_creature /
// gather_item / visit_zone / talk_to / accumulate_gold / etc.
// Renamed from achievement_criterion_types in mig 0226 because
// the catalog is shared by quests and achievements. target_schema /
// target_table encode where the polymorphic `target` resolves.
public sealed record ObjectiveTypeDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? TargetSchema,
    string? TargetTable,
    int SortOrder
);
