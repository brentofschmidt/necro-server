namespace Game.Core.Network;

// Who is on the other end of a connection. Produced by the network
// adapter at handshake time:
//
//   - dev / stub mode → parsed from "?as=player_N" query param
//   - prod / real auth → parsed from a validated Supabase JWT
//
// The world layer treats Subject as opaque — it's the stable id for "this
// principal" and is used to look up or create the PlayerSession. The
// adapter is what knows how to authenticate; the world is what knows
// what to do once authenticated.
public sealed record AuthContext(string Subject, string DisplayName);
