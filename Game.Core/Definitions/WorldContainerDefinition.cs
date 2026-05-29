using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.world_containers — chests, sacks, barrels.
// container_type / lock_type repointed to uuid FKs in mig 0215.
public sealed record WorldContainerDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    Guid? ContainerType,
    Guid? LockType,
    int SlotCount,
    string? IconPath
);
