using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.auras_modifiers — auras↔modifiers junction
// (mig 0220).
public sealed record AurasModifierLink(
    Guid AuraId,
    Guid ModifierId,
    int SortOrder
);
