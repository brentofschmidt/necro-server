using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.quest_objectives — per-quest objective row
// (mig 0226). target is polymorphic — resolve via objective_type's
// target_schema/target_table.
public sealed record QuestObjectiveDefinition(
    Guid Id,
    Guid QuestId,
    Guid ObjectiveType,
    Guid? Target,
    int RequiredCount,
    string? Description,
    int SortOrder
);
