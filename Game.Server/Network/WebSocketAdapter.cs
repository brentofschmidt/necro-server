using System.Net.WebSockets;
using System.Text.RegularExpressions;
using Game.Core.Network;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Game.Server.Network;

// ASP.NET Core-backed WebSocket implementation of INetworkAdapter.
// Network threads only deserialize and forward; they never mutate game
// state. The world's tick loop drains the incoming queue at tick start.
public sealed class WebSocketAdapter : INetworkAdapter
{
    private static readonly Regex StubSubject = new(@"^player_\d+$", RegexOptions.Compiled);

    private readonly ILogger<WebSocketAdapter> _logger;
    private readonly IHostEnvironment _env;
    private readonly ConnectionRegistry _connections = new();

    public WebSocketAdapter(ILogger<WebSocketAdapter> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public event PacketReceivedHandler? PacketReceived;
    public event ConnectionOpenedHandler? ConnectionOpened;
    public event ConnectionClosedHandler? ConnectionClosed;

    // Phase 2: lifecycle methods exist to satisfy INetworkAdapter but the
    // ASP.NET host owns binding/disposal. They become meaningful when the
    // UDP adapter lands and owns its own listener.
    public Task StartAsync(int port) => Task.CompletedTask;
    public Task StopAsync() => Task.CompletedTask;

    public async Task SendAsync(string connectionId, ReadOnlyMemory<byte> payload)
    {
        var socket = _connections.Get(connectionId);
        if (socket is null || socket.State != WebSocketState.Open) return;
        await socket.SendAsync(payload, WebSocketMessageType.Text, endOfMessage: true, CancellationToken.None);
    }

    public async Task CloseAsync(string connectionId, string reason)
    {
        var socket = _connections.Get(connectionId);
        if (socket is null) return;
        if (socket.State != WebSocketState.Open && socket.State != WebSocketState.CloseReceived) return;
        try
        {
            // PolicyViolation is the right code for "you authenticated but
            // the world rejected your session" (vs. NormalClosure which the
            // client uses to mean "I'm leaving"). Receive loop's finally
            // block handles registry removal + ConnectionClosed event.
            await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, reason, CancellationToken.None);
        }
        catch (WebSocketException ex)
        {
            _logger.LogDebug(ex, "ws close raced on {ConnId}", connectionId);
        }
    }

    public async Task HandleConnection(HttpContext ctx)
    {
        if (!ctx.WebSockets.IsWebSocketRequest)
        {
            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        // Authenticate BEFORE accepting the websocket upgrade so a rejected
        // connection sees an HTTP 401 instead of an immediate ws close.
        var auth = TryAuthenticate(ctx);
        if (auth is null)
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await ctx.Response.WriteAsync("auth required (dev: ?as=player_N)");
            return;
        }

        using var socket = await ctx.WebSockets.AcceptWebSocketAsync();
        var connectionId = Guid.NewGuid().ToString("N");
        _connections.Add(connectionId, socket);
        ConnectionOpened?.Invoke(connectionId, auth);
        _logger.LogInformation("ws open {ConnId} as {Subject} (total={Total})", connectionId, auth.Subject, _connections.Count);

        var buffer = new byte[8 * 1024];
        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer, ctx.RequestAborted);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "client close", CancellationToken.None);
                    break;
                }
                // Phase 2 only supports single-frame messages. Larger payloads
                // come in phase 3+ with snapshot deltas — at which point we
                // grow a per-connection re-assembly buffer.
                if (!result.EndOfMessage)
                {
                    _logger.LogWarning("multi-frame message from {ConnId}, ignoring", connectionId);
                    continue;
                }
                var slice = new ReadOnlyMemory<byte>(buffer, 0, result.Count);
                PacketReceived?.Invoke(connectionId, slice);
            }
        }
        catch (WebSocketException ex)
        {
            _logger.LogWarning(ex, "ws error on {ConnId}", connectionId);
        }
        catch (OperationCanceledException) { /* shutdown */ }
        finally
        {
            _connections.Remove(connectionId);
            ConnectionClosed?.Invoke(connectionId);
            _logger.LogInformation("ws close {ConnId} (total={Total})", connectionId, _connections.Count);
        }
    }

    private AuthContext? TryAuthenticate(HttpContext ctx)
    {
        // Dev-only stub: ?as=player_N. Locked behind environment check so
        // production builds can never accidentally accept it.
        if (_env.IsDevelopment() && ctx.Request.Query.TryGetValue("as", out var asValues))
        {
            var subject = asValues.ToString();
            if (StubSubject.IsMatch(subject))
            {
                var n = subject.Substring("player_".Length);
                return new AuthContext(subject, $"Player {n}");
            }
            _logger.LogWarning("malformed stub auth ?as={Value}", subject);
        }

        // Future: parse JWT from a header or query, validate against
        // Supabase JWKS, build AuthContext from the verified claims.
        return null;
    }
}
