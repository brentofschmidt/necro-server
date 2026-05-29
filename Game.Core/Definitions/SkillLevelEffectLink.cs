using System;
using System.Text.Json;

namespace Game.Core.Definitions;

// Mirrors necro_content.skill_level_effects — per-level effect
// grants for a skill (mig 0228). Inner payload remains jsonb in
// the DB; surfaced here as a raw JsonElement until the loader
// projects it into the unified effects shape.
public sealed record SkillLevelEffectLink(
    Guid Id,
    Guid SkillId,
    int Level,
    JsonElement Effect,
    int SortOrder
);
