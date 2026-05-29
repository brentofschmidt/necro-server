using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using System.Text.Json;
using Game.Core.Network;
using Game.Core.Packets;
using Game.Core.World;
using Game.Server.Handlers;
using Game.Server.Metrics;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace Game.Server.World;

// One per realm. Owns its tick loop on its own task. The tick thread is
// the only thread that mutates this realm's state — network threads
// only enqueue.
public sealed class WorldInstance
{
    private const int TickHz = 20;
    private const int TickMs = 1000 / TickHz;

    private readonly RealmConfig _config;
    private readonly ILogger<WorldInstance> _logger;
    private readonly INetworkAdapter _network;

    private readonly ConcurrentQueue<(string ConnId, ReadOnlyMemory<byte> Payload)> _inbox = new();
    private readonly ConcurrentQueue<ConnectionEvent> _connectionEvents = new();

    // Tick-thread state — never touched from network threads.
    private readonly Dictionary<string, PlayerSession> _sessionsByConnection = new();
    private readonly Dictionary<string, PlayerSession> _sessionsBySubject = new();
    private readonly Dictionary<EntityId, PlayerSession> _sessionsByEntity = new();
    private readonly SpatialGrid _grid = new();
    private long _tickCount;

    private CancellationTokenSource? _cts;
    private Task? _tickTask;

    // Pre-bound per-realm metric children so the hot paths don't pay the
    // label-lookup cost every call. The realm_id label is stable for the
    // lifetime of this instance.
    private readonly Histogram.Child _mTickDuration;
    private readonly Counter.Child _mTickBudgetExceeded;
    private readonly Gauge.Child _mPlayersConnected;
    private readonly Counter.Child _mPacketsReceived;
    private readonly Counter.Child _mPacketsSent;
    private readonly Counter.Child _mUnhandledExceptions;

    public WorldInstance(RealmConfig config, INetworkAdapter network, ILogger<WorldInstance> logger)
    {
        _config = config;
        _network = network;
        _logger = logger;

        var realmLabel = _config.RealmId.ToString();
        _mTickDuration = GameMetrics.TickDurationSeconds.WithLabels(realmLabel);
        _mTickBudgetExceeded = GameMetrics.TickBudgetExceededTotal.WithLabels(realmLabel);
        _mPlayersConnected = GameMetrics.PlayersConnected.WithLabels(realmLabel);
        _mPacketsReceived = GameMetrics.PacketsReceivedTotal.WithLabels(realmLabel);
        _mPacketsSent = GameMetrics.PacketsSentTotal.WithLabels(realmLabel);
        _mUnhandledExceptions = GameMetrics.UnhandledExceptionsTotal.WithLabels(realmLabel);
    }

    public RealmConfig Config => _config;
    internal IReadOnlyDictionary<string, PlayerSession> SessionsByConnection => _sessionsByConnection;
    internal IReadOnlyDictionary<EntityId, PlayerSession> SessionsByEntity => _sessionsByEntity;
    internal SpatialGrid Grid => _grid;
    internal long CurrentTick => _tickCount;
    internal INetworkAdapter Network => _network;
    internal ILogger Logger => _logger;

    public void EnqueuePacket(string connectionId, ReadOnlyMemory<byte> payload)
    {
        // Copy because the buffer is reused by the caller after this returns.
        var copy = payload.ToArray();
        _inbox.Enqueue((connectionId, copy));
    }

    // Called by the network adapter (network thread) when a connection
    // successfully authenticates. We push it onto an event queue and let
    // the tick thread do the actual session create — keeps the
    // "tick thread is the only writer" invariant.
    public void OnConnectionOpened(string connectionId, AuthContext auth)
    {
        _connectionEvents.Enqueue(new ConnectionEvent(ConnectionEventKind.Opened, connectionId, auth));
    }

    public void OnConnectionClosed(string connectionId)
    {
        _connectionEvents.Enqueue(new ConnectionEvent(ConnectionEventKind.Closed, connectionId, null));
    }

    public void Start()
    {
        _cts = new CancellationTokenSource();
        _tickTask = Task.Run(() => TickLoop(_cts.Token));
        _logger.LogInformation("realm {RealmId} ({Name}) tick loop started at {Hz}Hz", _config.RealmId, _config.Name, TickHz);
    }

    public async Task StopAsync()
    {
        _cts?.Cancel();
        if (_tickTask is not null) await _tickTask;
        _logger.LogInformation("realm {RealmId} stopped", _config.RealmId);
    }

    private async Task TickLoop(CancellationToken token)
    {
        var nextHeartbeat = DateTimeOffset.UtcNow.AddSeconds(1);
        var sw = new Stopwatch();
        while (!token.IsCancellationRequested)
        {
            sw.Restart();
            try { Tick(); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "tick {N} threw", _tickCount);
                _mUnhandledExceptions.Inc();
            }
            sw.Stop();
            _tickCount++;

            var elapsedSec = sw.Elapsed.TotalSeconds;
            _mTickDuration.Observe(elapsedSec);
            if (elapsedSec > TickMs / 1000.0) _mTickBudgetExceeded.Inc();

            if (DateTimeOffset.UtcNow >= nextHeartbeat)
            {
                _logger.LogDebug("tick {N} players={P}", _tickCount, _sessionsByConnection.Count);
                nextHeartbeat = DateTimeOffset.UtcNow.AddSeconds(1);
            }

            var sleep = TickMs - (int)sw.Elapsed.TotalMilliseconds;
            if (sleep > 0)
            {
                try { await Task.Delay(sleep, token); }
                catch (TaskCanceledException) { break; }
            }
        }
    }

    private void Tick()
    {
        // 1. Process connection lifecycle events (open/close) before
        //    anything else this tick — so packets from a brand-new
        //    connection on the same tick find the session ready.
        while (_connectionEvents.TryDequeue(out var ev))
        {
            if (ev.Kind == ConnectionEventKind.Opened && ev.Auth is not null)
                HandleConnectionOpened(ev.ConnectionId, ev.Auth);
            else if (ev.Kind == ConnectionEventKind.Closed)
                HandleConnectionClosed(ev.ConnectionId);
        }

        // 2. Drain inbox and dispatch packets to handlers (stamping
        //    DesiredMove etc. on sessions).
        while (_inbox.TryDequeue(out var entry))
        {
            DispatchPacket(entry.ConnId, entry.Payload);
        }

        // 3. Simulate. MovementSystem applies DesiredMove → Position and
        //    keeps the spatial grid in sync.
        MovementSystem.Tick(TickMs / 1000f, _sessionsByConnection.Values, _grid);

        // 4. Snapshot fan-out — every visible entity, every tick.
        SnapshotSystem.Tick(_tickCount, _sessionsByEntity, _sessionsByConnection.Values, _grid, this);

        // Future phases:
        //   - AI, combat, buffs, regen, respawns
        //   - Delta-since-last-acked snapshots
        //   - Mark dirty for persistence
    }

    private void HandleConnectionOpened(string connectionId, AuthContext auth)
    {
        // Stub-auth convention: "player_N" → EntityId(N). Replace when
        // real Supabase auth lands.
        if (!TryParsePlayerEntity(auth.Subject, out var entityId))
        {
            _logger.LogWarning("rejecting conn {ConnId}: unknown subject shape {Subject}", connectionId, auth.Subject);
            // Fire-and-forget: don't block the tick on a socket close.
            // ConnectionClosed will arrive via the event queue later and
            // be a no-op since we never added a session for this conn.
            _ = _network.CloseAsync(connectionId, "unknown subject");
            return;
        }

        // If this subject is already logged in on another connection, kick
        // the old one. Latest login wins — standard MMO behavior.
        if (_sessionsBySubject.TryGetValue(auth.Subject, out var existing))
        {
            _logger.LogInformation("kicking previous session {OldConn} for subject {Subject}", existing.ConnectionId, auth.Subject);
            _ = _network.CloseAsync(existing.ConnectionId, "kicked by new login");
            RemoveSession(existing);
        }

        var spawn = Vector3.Zero;
        var session = new PlayerSession(entityId, connectionId, auth.Subject, auth.DisplayName, spawn);
        _sessionsByConnection[connectionId] = session;
        _sessionsBySubject[auth.Subject] = session;
        _sessionsByEntity[entityId] = session;
        _grid.Update(entityId, spawn);
        _mPlayersConnected.Inc();

        _logger.LogInformation("session created: {Subject} → {EntityId} at {Pos}", auth.Subject, entityId, spawn);

        // Welcome packet so the client knows which entity is itself.
        var welcome = new WelcomePacket(entityId, session.DisplayName, _config.RealmId, _config.Name, spawn);
        SendTo(connectionId, welcome);
    }

    private void HandleConnectionClosed(string connectionId)
    {
        if (!_sessionsByConnection.TryGetValue(connectionId, out var session)) return;
        RemoveSession(session);
    }

    private void RemoveSession(PlayerSession session)
    {
        _sessionsByConnection.Remove(session.ConnectionId);
        _sessionsBySubject.Remove(session.Subject);
        _sessionsByEntity.Remove(session.Id);
        _grid.Remove(session.Id);
        _mPlayersConnected.Dec();
        _logger.LogInformation("session removed: {Subject} ({EntityId})", session.Subject, session.Id);
    }

    private static bool TryParsePlayerEntity(string subject, out EntityId id)
    {
        const string prefix = "player_";
        if (subject.StartsWith(prefix, StringComparison.Ordinal)
            && uint.TryParse(subject.AsSpan(prefix.Length), out var n))
        {
            id = new EntityId(n);
            return true;
        }
        id = EntityId.None;
        return false;
    }

    internal void SendTo<T>(string connectionId, T packet) where T : IPacket
    {
        var envelope = PacketEnvelope.Of(packet);
        var bytes = JsonSerializer.SerializeToUtf8Bytes(envelope);
        _mPacketsSent.Inc();
        _ = _network.SendAsync(connectionId, bytes);
    }

    private void DispatchPacket(string connectionId, ReadOnlyMemory<byte> payload)
    {
        _mPacketsReceived.Inc();
        PacketEnvelope? env;
        try
        {
            env = JsonSerializer.Deserialize<PacketEnvelope>(payload.Span);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "malformed packet from {ConnId} ({Len} bytes)", connectionId, payload.Length);
            return;
        }

        if (env is null) { _logger.LogWarning("null envelope from {ConnId}", connectionId); return; }

        _sessionsByConnection.TryGetValue(connectionId, out var session);

        switch (env.Type)
        {
            case "ping":
                PingHandler.Handle(env.As<PingPacket>(), connectionId, _network, _logger);
                break;
            case "move_intent":
                if (session is null)
                {
                    _logger.LogWarning("move_intent from unauth conn {ConnId}", connectionId);
                    break;
                }
                MoveIntentHandler.Handle(env.As<MoveIntentPacket>(), session, _logger);
                break;
            default:
                _logger.LogWarning("unknown packet type '{Type}' from {ConnId}", env.Type, connectionId);
                break;
        }
    }

    private enum ConnectionEventKind { Opened, Closed }
    private sealed record ConnectionEvent(ConnectionEventKind Kind, string ConnectionId, AuthContext? Auth);
}
