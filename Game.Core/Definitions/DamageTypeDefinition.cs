using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.damage_types — physical / fire / cold / etc.
public sealed record DamageTypeDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    int SortOrder
);
