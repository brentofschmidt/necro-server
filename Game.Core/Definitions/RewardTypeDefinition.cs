using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.reward_types — item / title / mount / pet /
// heirloom / skill_xp / gold / xp / currency (mig 0279). Polymorphic
// catalog; reward_id on achievement_rewards interprets via
// target_schema/target_table (null for quantity-only rewards).
public sealed record RewardTypeDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? TargetSchema,
    string? TargetTable,
    int SortOrder
);
