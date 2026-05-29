using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.aura_types — buff / debuff / blessing /
// curse / poison / stance. is_harmful encodes the harm/help bit
// here, replacing the boolean that used to sit on auras (mig 0266).
public sealed record AuraTypeDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    bool IsHarmful,
    int SortOrder
);
