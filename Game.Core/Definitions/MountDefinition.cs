using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.mounts — collectible mounts. `family`
// dropped and `type` repointed to creature_types in mig 0259-0261.
public sealed record MountDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string Description,
    Guid? Type,
    Guid Rarity,
    float Speed,
    string? IconPath
);
