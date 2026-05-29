using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.doors — placeable doors with optional
// lock type (mig 0100, lock_type FK added in 0210/0215).
public sealed record DoorDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    Guid? LockType,
    string? IconPath
);
