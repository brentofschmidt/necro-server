namespace Game.Core.World;

// Stable id for any in-world entity (player, mob, item). uint so we can fit
// 4B entity-lifetimes per realm before wrapping; in practice we reset on
// realm restart and never come close.
//
// record struct = value-typed, equatable, cheap to pass around. Strongly
// typed so the compiler stops you from passing an item id where a player
// id is expected, once we split these later.
public readonly record struct EntityId(uint Value)
{
    public static EntityId None => default;
    public override string ToString() => $"E{Value}";
}
