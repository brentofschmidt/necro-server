using Game.Core.Packets;
using Game.Core.World;
using Game.Server.World;
using Microsoft.Extensions.Logging;

namespace Game.Server.Handlers;

internal static class MoveIntentHandler
{
    public static void Handle(MoveIntentPacket packet, PlayerSession session, ILogger logger)
    {
        // Normalize the requested direction. Clients that send length>1
        // (deliberately or by bug) are clamped to unit length so they
        // can't bypass the speed cap. Length<threshold means "stop."
        var (dir, _) = packet.Direction.Normalized();
        session.DesiredMove = dir;
        session.Facing = packet.Facing;

        // Reject out-of-order or stale sequences. MoveIntent.Sequence is
        // monotonic per client; gaps are fine (dropped packets) but a
        // smaller sequence than what we've already processed means a
        // late-arriving reorder — discard.
        if (packet.Sequence > session.LastProcessedSequence)
        {
            session.LastProcessedSequence = packet.Sequence;
        }
        else
        {
            logger.LogDebug("stale move seq {Seq} from {Entity} (have {Have})",
                packet.Sequence, session.Id, session.LastProcessedSequence);
        }
    }
}
