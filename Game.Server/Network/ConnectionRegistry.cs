using System.Net.WebSockets;

namespace Game.Server.Network;

internal sealed class ConnectionRegistry
{
    private readonly Dictionary<string, WebSocket> _connections = new();
    private readonly object _lock = new();

    public void Add(string id, WebSocket socket)
    {
        lock (_lock) { _connections[id] = socket; }
    }

    public void Remove(string id)
    {
        lock (_lock) { _connections.Remove(id); }
    }

    public WebSocket? Get(string id)
    {
        lock (_lock) { return _connections.TryGetValue(id, out var s) ? s : null; }
    }

    public int Count
    {
        get { lock (_lock) { return _connections.Count; } }
    }
}
