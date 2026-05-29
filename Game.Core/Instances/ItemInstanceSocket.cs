using System;

namespace Game.Core.Instances;

// Mirrors necro_player.item_instance_sockets — one row per socket
// slot on an item instance (mig 0249). SocketedItemId NULL = empty
// socket. The socketed thing is itself an item (gems became items
// in mig 0238).
public sealed record ItemInstanceSocket(
    Guid ItemInstanceId,
    short SlotIndex,
    Guid? SocketedItemId
);
