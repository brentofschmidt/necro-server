using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.auras — buff / debuff / blessing / curse /
// poison / stance catalog. Per-tick periodic payload moved to
// aura_effects (mig 0265); is_harmful replaced by type FK (0266);
// modifiers live in auras_modifiers junction.
public sealed record AuraDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    string? IconPath,
    float Duration,
    Guid? Type,
    int MaxStacks,
    float? TickInterval
);
