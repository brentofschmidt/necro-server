using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.resource_nodes — gatherable nodes (ore /
// tree / herb / etc.). resource_node_types catalog added in 0214.
public sealed record ResourceNodeDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    Guid? Type,
    Guid? RequiredSkillId,
    int RequiredSkillLevel,
    Guid? LootTableId,
    string? IconPath
);
