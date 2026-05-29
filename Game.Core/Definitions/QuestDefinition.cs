using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.quests — quest catalog. Objectives are
// in quest_objectives (mig 0226); reward_items in quests_reward_items
// (mig 0223/0224). source_type for who hands it out (mig 0200-ish).
public sealed record QuestDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    Guid? Zone,
    Guid? SourceType,
    Guid? SourceId,
    int RewardXp,
    int RewardGold
);
