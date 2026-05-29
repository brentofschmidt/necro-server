using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.lock_types — simple / wooden / iron / etc.
// (mig 0210). Used by doors + world_containers.
public sealed record LockTypeDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    int Difficulty,
    int SortOrder
);
