using System;
using System.Text.Json;

namespace Game.Core.Network;

public sealed class PacketEnvelope
{
    public string Type { get; set; } = "";
    public JsonElement Data { get; set; }

    public static PacketEnvelope Of<T>(T packet) where T : IPacket
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(packet);
        using var doc = JsonDocument.Parse(bytes);
        return new PacketEnvelope { Type = packet.Type, Data = doc.RootElement.Clone() };
    }

    public T As<T>() where T : IPacket
    {
        return JsonSerializer.Deserialize<T>(Data.GetRawText())
            ?? throw new InvalidOperationException($"Failed to deserialize packet as {typeof(T).Name}");
    }
}

public interface IPacket
{
    string Type { get; }
}
