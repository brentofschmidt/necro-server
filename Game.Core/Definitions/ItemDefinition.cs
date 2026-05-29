using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.items — the master item catalog. After
// migration 0275 bag-specific columns moved to item_bag_specs (1:1
// extension); after 0249 per-instance state moved to item_instances
// junctions; after 0220 modifiers live in items_modifiers junction.
public sealed record ItemDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string Description,
    Guid ItemSubclass,
    Guid Rarity,
    Guid? ArmorClass,
    Guid? RequiredSkillId,
    int RequiredSkillLevel,
    bool IsStackable,
    int MaxStackSize,
    float Weight,
    float? WeaponSpeed,
    bool IsConsumable,
    float? ConsumableCooldown,
    bool IsCraftable,
    bool IsPlaceable,
    int SocketCount,
    Guid InventorySlot,
    short? Tier,
    string? DisplayColor,
    string? IconPath,
    string? PlacedAsset,
    string? PickupSoundKey,
    string? EquipSoundKey,
    string? UseSoundKey
);
