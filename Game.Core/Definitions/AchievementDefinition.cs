using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.achievements — achievement catalog.
// Criteria live in achievement_criteria; rewards in
// achievement_rewards (polymorphic via reward_types, mig 0279).
public sealed record AchievementDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string Description,
    Guid? Category,
    int Points,
    string? IconPath
);
