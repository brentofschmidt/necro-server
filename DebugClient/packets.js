// JS mirror of Game.Core packet shapes. Keep field names and `type`
// strings in sync with the C# records by hand for now. When packet
// shapes stabilize we can generate this from a single source.

const Packets = {
  ping: (clientTimeMs) => ({
    Type: 'ping',
    Data: { ClientTimeMs: clientTimeMs },
  }),

  moveIntent: (dirX, dirZ, facing, sequence) => ({
    Type: 'move_intent',
    Data: {
      Direction: { X: dirX, Y: 0, Z: dirZ },
      Facing: facing,
      Sequence: sequence,
    },
  }),
};
