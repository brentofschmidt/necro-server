using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.rarities — WoW-style tier ladder
// (trash / common / uncommon / rare / epic / legendary / mythic).
public sealed record RarityDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    string? DisplayColor,
    int SortOrder
);
