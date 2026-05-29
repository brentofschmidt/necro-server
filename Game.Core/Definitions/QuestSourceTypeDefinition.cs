using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.quest_source_types — npc / item / world /
// etc. Polymorphic catalog with target_schema/target_table fields
// (same pattern as spawn_types, reward_types).
public sealed record QuestSourceTypeDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? TargetSchema,
    string? TargetTable,
    int SortOrder
);
