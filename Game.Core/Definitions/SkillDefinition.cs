using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.skills — per-skill progression (weapon
// proficiencies, gathering, crafting, magic schools, armor classes).
// Per-level effects normalized into skill_level_effects (mig 0228).
public sealed record SkillDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    Guid? Category,
    string? Description,
    int MaxLevel
);
