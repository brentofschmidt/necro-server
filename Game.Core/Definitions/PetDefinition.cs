using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.pets — collectible non-combat pets.
// pet_families merged into creature_types in mig 0268-0269.
public sealed record PetDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string Description,
    Guid? Type,
    Guid Rarity,
    string? IconPath
);
