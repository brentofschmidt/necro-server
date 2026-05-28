# Server Architecture — Walkthrough

A guided tour of how this server actually works, written for someone who knows
C# from Unity but hasn't built a .NET server before. Read top-to-bottom; each
section assumes the one above it.

For the *design* (the why) see `necro-client/design/SERVER_ARCHITECTURE.md`.
This doc is the *how* — concrete code paths in this repo.

---

## 1. The four buildings

```
┌─────────────────────────────────────────────────────────────────────┐
│                                                                     │
│   Game.Core                                                         │
│   (.NET Standard 2.1 library)                                       │
│                                                                     │
│   The "contract." Packet shapes, interfaces, pure formulas.         │
│   No Unity refs, no server refs — just plain C#.                    │
│                                                                     │
│   Built once → DLL copied into BOTH consumers below.                │
│                                                                     │
└──────────┬───────────────────────────────────────────────┬──────────┘
           │                                               │
           │ references                                    │ references
           │                                               │
           ▼                                               ▼
┌──────────────────────────┐                  ┌────────────────────────┐
│                          │                  │                        │
│   Game.Server            │                  │   Unity client         │
│   (.NET 8 console app)   │   ◄──────────►   │   (separate repo)      │
│                          │     WebSocket    │                        │
│   The authoritative      │     ws://...     │   Sends intents,       │
│   simulation.            │     :7777/ws     │   renders snapshots.   │
│                          │                  │                        │
└──────────┬───────────────┘                  └────────────────────────┘
           │
           │ also serves
           │
           ▼
┌──────────────────────────┐
│                          │
│   DebugClient            │
│   (static HTML+JS)       │
│                          │
│   Browser-based fake     │
│   client. Goes through   │
│   the SAME handlers as   │
│   Unity. http://...:7777 │
│   /debug                 │
│                          │
└──────────────────────────┘
```

**Key idea:** `Game.Core.dll` is the contract that both Unity and the server
agree on. Define a packet there once and both sides serialize/deserialize it
identically. That's why this is a three-project solution and not just one.

---

## 2. What happens when you start the server

`dotnet run --project Game.Server` triggers `Program.cs`, which does five
things in order:

```
1. Build an ASP.NET Core "web host"  ← this is the HTTP + WebSocket plumbing
2. Register services in DI container ← WebSocketAdapter, WorldInstance
3. Wire events: adapter.PacketReceived → world.EnqueuePacket
4. Map routes: /ws (websocket), /debug (static files), /health (json)
5. world.Start() → spins up the tick loop on its own Task
6. app.RunAsync(...) → block forever, listening on http://localhost:7777
```

If you've only seen Unity C#, two things will look unfamiliar:

- **Dependency injection (DI).** `builder.Services.AddSingleton<X>()` says
  "when anything asks for an `X`, give them this same instance." This is the
  ASP.NET equivalent of Unity's "drag a reference into the Inspector." Code
  asks for what it needs in its constructor, the framework wires it up.
- **`async Task`.** Servers do a LOT of waiting on I/O (network reads, file
  reads, eventually Supabase calls). `async` lets one thread serve many
  connections without blocking. Unity has `async` too but you rarely need it
  on the game-thread side; here it's everywhere on the I/O side.

---

## 3. The threading model (THE rule)

There are **two kinds of threads** in this server, and they must not touch
the same data:

```
   ┌─────────────────────────────────┐         ┌─────────────────────────┐
   │  Network threads (many)         │         │  Tick thread (one)      │
   │                                 │         │                         │
   │  - Owned by ASP.NET Core        │         │  - Owned by Task.Run    │
   │  - One read loop per WebSocket  │         │    in WorldInstance     │
   │  - Receives bytes, deserializes │         │  - Wakes every 50 ms    │
   │  - Calls _inbox.Enqueue(...)    │         │  - Drains the inbox     │
   │  - Returns immediately          │         │  - Mutates world state  │
   │                                 │         │  - Sleeps until 50 ms   │
   │                                 │         │    elapsed              │
   └────────────────┬────────────────┘         └────────────┬────────────┘
                    │                                       │
                    │ enqueue(packet)                       │ dequeue(packet)
                    │                                       │
                    ▼                                       ▼
              ┌─────────────────────────────────────────────────┐
              │  ConcurrentQueue<(connId, payload)>   "inbox"   │
              │  (thread-safe; the ONLY shared mutable thing)   │
              └─────────────────────────────────────────────────┘
```

**Rule:** Network threads never mutate game state. They put work in the
inbox; the tick thread takes it out.

Why this matters: as long as the tick thread is the only thing touching world
state, you never need locks inside game logic. Inside a tick the simulation
is effectively single-threaded — much easier to reason about than locking
every shared dict.

The relevant file is `Game.Server/World/WorldInstance.cs`:

- `EnqueuePacket(...)` — called by network threads. Just copies bytes into a
  `ConcurrentQueue` and returns.
- `TickLoop(...)` — runs on its own Task. Every 50 ms: drain the queue,
  dispatch each packet, sleep the remainder.

---

## 4. A packet's full journey (Ping → Pong)

This is the only packet implemented today, but the path it walks is the path
every future packet will walk. Read it carefully — once you've seen this,
adding a new packet type is a 5-line change.

```
 BROWSER (DebugClient)              GAME.SERVER                    GAME.CORE
 ──────────────────                 ───────────                    ─────────

 You click "Send Ping" in
 DebugClient/index.html
        │
        ▼
 app.js:
   ws.send(JSON.stringify({
     type: "ping",
     data: { clientTimeMs: Date.now() }
   }))
        │
        │  bytes over WebSocket
        ▼
                          ┌──────────────────────────────────┐
                          │ WebSocketAdapter.HandleConnection│
                          │                                  │
                          │   socket.ReceiveAsync(buffer)    │
                          │   → got bytes                    │
                          │                                  │
                          │   PacketReceived?.Invoke(        │
                          │     connectionId, bytes)         │
                          └────────────────┬─────────────────┘
                                           │
                                           ▼ (event)
                          ┌──────────────────────────────────┐
                          │ Program.cs wired this up:        │
                          │   adapter.PacketReceived +=      │
                          │     (id, bytes) => world         │
                          │       .EnqueuePacket(id, bytes); │
                          └────────────────┬─────────────────┘
                                           │
                                           ▼
                          ┌──────────────────────────────────┐
                          │ WorldInstance.EnqueuePacket      │
                          │                                  │
                          │   _inbox.Enqueue((id, bytes))    │
                          │                                  │
                          │   (network thread RETURNS HERE   │
                          │    — it does NOT process)        │
                          └──────────────────────────────────┘

                            ... ≤ 50ms later, on the TICK thread ...

                          ┌──────────────────────────────────┐
                          │ WorldInstance.TickLoop / Tick    │
                          │                                  │
                          │   while (_inbox.TryDequeue(..))  │
                          │     DispatchPacket(...)          │
                          └────────────────┬─────────────────┘
                                           │
                                           ▼
                          ┌──────────────────────────────────┐
                          │ WorldInstance.DispatchPacket     │
                          │                                  │            ┌──────────────────┐
                          │   env = Deserialize<             ├───────────►│ PacketEnvelope   │
                          │     PacketEnvelope>(bytes)       │            │ { Type, Data }   │
                          │                                  │            └──────────────────┘
                          │   switch (env.Type) {            │
                          │     case "ping":                 │            ┌──────────────────┐
                          │       PingHandler.Handle(        ├───────────►│ PingPacket       │
                          │         env.As<PingPacket>(),    │            │ { ClientTimeMs } │
                          │         connId, _network, log)   │            └──────────────────┘
                          │   }                              │
                          └────────────────┬─────────────────┘
                                           │
                                           ▼
                          ┌──────────────────────────────────┐
                          │ PingHandler.Handle               │            ┌──────────────────┐
                          │                                  ├───────────►│ PongPacket       │
                          │   pong = new PongPacket(         │            │ { ClientTimeMs,  │
                          │     packet.ClientTimeMs,         │            │   ServerTimeMs } │
                          │     now())                       │            └──────────────────┘
                          │   bytes = Serialize(             │
                          │     PacketEnvelope.Of(pong))     │
                          │   _ = network.SendAsync(         │
                          │     connId, bytes)               │
                          └────────────────┬─────────────────┘
                                           │
                                           │  fire-and-forget
                                           ▼
                          ┌──────────────────────────────────┐
                          │ WebSocketAdapter.SendAsync       │
                          │                                  │
                          │   socket.SendAsync(payload, ...) │
                          └────────────────┬─────────────────┘
                                           │
                                           │  bytes over WebSocket
                                           ▼
 app.js:
   ws.onmessage = (e) => {
     const env = JSON.parse(e.data)
     if (env.type === "pong") show(...)
   }
        │
        ▼
 You see "pong RTT 4ms" in the DebugClient log.
```

A few things worth pointing out:

- **The envelope pattern.** Every packet has a `type` string and a `data`
  object. The server reads `type` first to decide which handler to call,
  then deserializes `data` into the right struct. This is how a single
  WebSocket carries N different packet kinds without confusion.
- **`_ = network.SendAsync(...)`.** The `_ =` says "I know this returns a
  Task, I don't want to await it." We deliberately do NOT block the tick on
  the network send — if a client's socket is slow, the tick must keep going
  for everyone else.
- **No allocation rules yet.** Phase 2 happily allocates byte arrays. This
  will get tightened once the tick budget is real, but premature
  optimization here would obscure the architecture.

---

## 5. File-by-file map

```
necro-server/
├── MMORPG.sln                        ← solution file (open this in Rider/VS)
│
├── Game.Core/                        ← the SHARED contract
│   ├── Game.Core.csproj              ← targets netstandard2.1 (Unity-compatible)
│   ├── IsExternalInit.cs             ← shim so `record` types compile on netstandard2.1
│   ├── Network/
│   │   ├── INetworkAdapter.cs        ← interface every transport implements (WS now, UDP later)
│   │   └── PacketEnvelope.cs         ← { Type, Data } wrapper + IPacket interface
│   └── Packets/
│       ├── PingPacket.cs             ← client → server
│       └── PongPacket.cs             ← server → client
│
├── Game.Server/                      ← the SIMULATION
│   ├── Game.Server.csproj            ← targets net8.0, references Game.Core
│   ├── Program.cs                    ← startup, DI wiring, route mapping
│   ├── appsettings.json              ← config (port, realm id)
│   ├── Network/
│   │   ├── WebSocketAdapter.cs       ← ASP.NET-backed INetworkAdapter (network threads)
│   │   └── ConnectionRegistry.cs     ← tiny thread-safe Dict<connId, WebSocket>
│   ├── Handlers/
│   │   └── PingHandler.cs            ← one static method per packet type
│   └── World/
│       ├── RealmConfig.cs            ← realm id + name (loaded from appsettings)
│       └── WorldInstance.cs          ← inbox + tick loop + dispatch (THE simulation thread)
│
├── DebugClient/                      ← browser fake-client
│   ├── index.html                    ← UI shell
│   ├── app.js                        ← WebSocket connect + UI + packet log
│   └── packets.js                    ← JS mirror of Game.Core packet shapes
│
└── tools/
    └── copy-dll-to-unity.ps1         ← `dotnet build Game.Core` then run this
                                        to ship Game.Core.dll into Unity
```

---

## 6. The Game.Core ↔ Unity bridge

Unity can't directly reference a .NET project from outside its `Assets/`
folder. Workaround: build `Game.Core` to a DLL and copy it into
`Assets/Plugins/`. Unity treats DLLs in `Plugins/` like any other assembly.

The script `tools/copy-dll-to-unity.ps1` automates this:

```pwsh
dotnet build Game.Core -c Release          # produces Game.Core.dll
./tools/copy-dll-to-unity.ps1              # copies it into Unity
```

Unity will detect the file change, reimport, and your `using Game.Core.Packets;`
in Unity scripts will resolve to the same code the server uses. **That's the
whole magic** — it's just a file copy, but the implication is that a packet
class authored once is now available verbatim on both sides.

---

## 7. How to add a new packet (the 5-step recipe)

Say you want a `ChatPacket` that carries a string. The full change is:

1. **Define the packet shape in `Game.Core/Packets/ChatPacket.cs`:**

   ```csharp
   public sealed record ChatPacket(string Body) : IPacket
   {
       [JsonIgnore] public string Type => "chat";
   }
   ```

2. **Write a handler in `Game.Server/Handlers/ChatHandler.cs`:**

   ```csharp
   internal static class ChatHandler
   {
       public static void Handle(ChatPacket packet, string connId,
                                 INetworkAdapter network, ILogger log)
       {
           log.LogInformation("chat from {ConnId}: {Body}", connId, packet.Body);
           // future: broadcast to recipients, persist, etc.
       }
   }
   ```

3. **Add a case to the dispatch switch in `WorldInstance.DispatchPacket`:**

   ```csharp
   case "chat":
       ChatHandler.Handle(env.As<ChatPacket>(), connectionId, _network, _logger);
       break;
   ```

4. **(If Unity needs it) rebuild and copy the DLL:**

   ```pwsh
   dotnet build Game.Core -c Release
   ./tools/copy-dll-to-unity.ps1
   ```

5. **(For browser testing) mirror the shape in `DebugClient/packets.js`** and
   add a button in `app.js` that sends one.

That's it. No DI registration needed — handlers are static; the switch is the
routing table.

---

## 8. Mental model summary

If you remember just four things:

1. **`Game.Core` is the shared contract.** Anything both Unity and the server
   need to agree on — packet shapes, formulas, enums — lives here.
2. **Network threads never touch game state.** They enqueue; the tick thread
   dequeues. Inbox is the only shared mutable thing.
3. **One tick thread per realm, 20 Hz.** Inside a tick, simulation is
   effectively single-threaded — no locks needed.
4. **Packets are envelopes.** `{ type, data }` over the wire; switch on
   `type` to pick a handler.

Phases 3+ in `necro-client/design/MIGRATION_PLAN.md` add more systems
(movement, combat, inventory) but every one of them slots into this same
shape: define packet in `Game.Core`, write handler in `Game.Server`, route
through the tick. The bones don't change.
