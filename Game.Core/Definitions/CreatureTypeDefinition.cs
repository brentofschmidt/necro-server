using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.creature_types — D&D-style category
// (beast / undead / dragon / fiend / etc.). Shared by creatures,
// mounts (after 0259/0260), and pets (after 0268/0269).
public sealed record CreatureTypeDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    int SortOrder
);
