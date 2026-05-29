using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.enchants_modifiers — enchants↔modifiers
// junction (mig 0220).
public sealed record EnchantsModifierLink(
    Guid EnchantId,
    Guid ModifierId,
    int SortOrder
);
