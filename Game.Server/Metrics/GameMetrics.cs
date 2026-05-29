using Prometheus;

namespace Game.Server.Metrics;

// Tier-1 metrics per design/server/METRICS_CATALOG.md — the minimum
// set that answers "is this realm alive and keeping up?". Process
// runtime metrics (GC, threadpool, etc.) come for free from
// prometheus-net's default collectors.
//
// All metrics carry a `realm_id` label except where the catalog
// explicitly says otherwise. Labels are kept low-cardinality —
// no per-player or per-packet-type fanout at this tier.
internal static class GameMetrics
{
    // Tick health
    public static readonly Histogram TickDurationSeconds = Prometheus.Metrics.CreateHistogram(
        "game_tick_duration_seconds",
        "Wall-clock time spent inside a tick (excluding the sleep until next tick).",
        new HistogramConfiguration
        {
            LabelNames = new[] { "realm_id" },
            // Buckets from METRICS_CATALOG: 1ms .. 250ms. Tick budget is 50ms.
            Buckets = new[] { 0.001, 0.005, 0.01, 0.025, 0.05, 0.1, 0.25 }
        });

    public static readonly Counter TickBudgetExceededTotal = Prometheus.Metrics.CreateCounter(
        "game_tick_budget_exceeded_total",
        "Ticks whose duration exceeded the 50ms budget.",
        new CounterConfiguration { LabelNames = new[] { "realm_id" } });

    // Population
    public static readonly Gauge PlayersConnected = Prometheus.Metrics.CreateGauge(
        "game_players_connected",
        "Authenticated players currently in-world.",
        new GaugeConfiguration { LabelNames = new[] { "realm_id" } });

    public static readonly Gauge ConnectionsActive = Prometheus.Metrics.CreateGauge(
        "game_connections_active",
        "Open transport connections (authenticated or not).",
        new GaugeConfiguration { LabelNames = new[] { "realm_id", "transport" } });

    // Packet throughput — no packet_type label at tier 1 (added at tier 2).
    public static readonly Counter PacketsReceivedTotal = Prometheus.Metrics.CreateCounter(
        "game_packets_received_total",
        "Total inbound packets dispatched by the world.",
        new CounterConfiguration { LabelNames = new[] { "realm_id" } });

    public static readonly Counter PacketsSentTotal = Prometheus.Metrics.CreateCounter(
        "game_packets_sent_total",
        "Total outbound packets handed to the network adapter.",
        new CounterConfiguration { LabelNames = new[] { "realm_id" } });

    // Reliability
    public static readonly Counter UnhandledExceptionsTotal = Prometheus.Metrics.CreateCounter(
        "game_unhandled_exceptions_total",
        "Exceptions caught at the tick-loop boundary (would otherwise crash the realm).",
        new CounterConfiguration { LabelNames = new[] { "realm_id" } });
}
