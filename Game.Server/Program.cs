using Game.Server.Metrics;
using Game.Server.Network;
using Game.Server.World;
using Microsoft.Extensions.FileProviders;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(o =>
{
    o.SingleLine = true;
    o.TimestampFormat = "HH:mm:ss.fff ";
    o.IncludeScopes = false;
});

builder.Services.AddSingleton<WebSocketAdapter>();
builder.Services.AddSingleton<WorldInstance>(sp =>
{
    var config = builder.Configuration.GetSection("Server");
    // Default to Mortis (the seeded dev realm) when no RealmId is configured.
    var defaultRealmId = Guid.Parse("e846aba4-67df-4bdd-97d7-d5d2e3625af1");
    var realm = new RealmConfig(
        RealmId: config.GetValue<Guid>("RealmId", defaultRealmId),
        Name: config.GetValue<string>("RealmName") ?? "Mortis");
    return new WorldInstance(
        realm,
        sp.GetRequiredService<WebSocketAdapter>(),
        sp.GetRequiredService<ILogger<WorldInstance>>());
});

var app = builder.Build();
var port = app.Configuration.GetValue<int?>("Server:Port") ?? 7777;
var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Wire network → world inbox.
var adapter = app.Services.GetRequiredService<WebSocketAdapter>();
var world = app.Services.GetRequiredService<WorldInstance>();
var connectionsActive = GameMetrics.ConnectionsActive.WithLabels(world.Config.RealmId.ToString(), "ws");
adapter.PacketReceived += (connId, payload) => world.EnqueuePacket(connId, payload);
adapter.ConnectionOpened += (connId, auth) =>
{
    connectionsActive.Inc();
    world.OnConnectionOpened(connId, auth);
};
adapter.ConnectionClosed += connId =>
{
    connectionsActive.Dec();
    world.OnConnectionClosed(connId);
};

// Serve DebugClient/ from the Necro/ root, walking up from the binary location.
var debugClient = FindDebugClient(AppContext.BaseDirectory);
if (debugClient is not null)
{
    app.UseDefaultFiles(new DefaultFilesOptions
    {
        FileProvider = new PhysicalFileProvider(debugClient),
        RequestPath = "/debug",
        DefaultFileNames = new[] { "index.html" }
    });
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(debugClient),
        RequestPath = "/debug"
    });
    logger.LogInformation("serving DebugClient from {Path} at /debug", debugClient);
}
else
{
    logger.LogWarning("DebugClient folder not found; /debug will 404");
}

app.UseWebSockets();
app.Map("/ws", async ctx => await adapter.HandleConnection(ctx));
app.MapGet("/", () => Results.Redirect("/debug"));
app.MapGet("/health", () => Results.Ok(new { realm = world.Config.RealmId, name = world.Config.Name }));
app.MapMetrics();

world.Start();
logger.LogInformation("realm {RealmId} ready on http://localhost:{Port}", world.Config.RealmId, port);

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    logger.LogInformation("shutdown requested");
    world.StopAsync().GetAwaiter().GetResult();
});

await app.RunAsync($"http://localhost:{port}");

static string? FindDebugClient(string startDir)
{
    var dir = new DirectoryInfo(startDir);
    while (dir is not null)
    {
        var candidate = Path.Combine(dir.FullName, "DebugClient");
        if (Directory.Exists(candidate)) return candidate;
        dir = dir.Parent;
    }
    return null;
}
