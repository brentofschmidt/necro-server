using System;

namespace Game.Core.Definitions;

// Mirrors what WAS necro_content.achievement_criterion_types — renamed
// to objective_types in mig 0226. Kept as a thin alias record so
// existing callers that still think in achievement-criterion terms
// can use a name that matches the domain. Same shape as
// ObjectiveTypeDefinition.
public sealed record AchievementCriterionTypeDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? TargetSchema,
    string? TargetTable,
    int SortOrder
);
