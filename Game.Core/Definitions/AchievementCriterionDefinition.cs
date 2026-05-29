using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.achievement_criteria — per-achievement
// criterion. criterion_type renamed to objective_type in mig 0226
// (shared catalog with quests). target is polymorphic — resolve via
// objective_type's target_schema/target_table.
public sealed record AchievementCriterionDefinition(
    Guid Id,
    Guid AchievementId,
    Guid ObjectiveType,
    Guid? Target,
    int RequiredCount,
    string? Description,
    int SortOrder
);
