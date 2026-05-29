using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.enchants — applied enchantments (added via
// enchanting profession). Modifiers via enchants_modifiers junction.
public sealed record EnchantDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    Guid[] AppliesToClasses,
    Guid[]? AppliesToSubclasses,
    string? IconPath
);
