using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.npcs — non-combat world entities (vendors,
// questgivers, trainers, flavor NPCs).
public sealed record NpcDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    bool IsVendor,
    bool IsQuestgiver,
    bool IsTrainer,
    string? IconPath
);
