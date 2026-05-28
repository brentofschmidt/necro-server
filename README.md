# necro-server

Authoritative game server for the Necro MMORPG. .NET 8 console app; WebSocket+JSON transport now, UDP+MemoryPack later behind the same `INetworkAdapter`. Unity client lives separately at `C:\Users\brent\UnityProjects\Necro\`. Design docs live in the sibling `necro-client/design/` repo (`SERVER_ARCHITECTURE.md` is authoritative).

## Layout

```
necro-server/
├── MMORPG.sln
├── Game.Core/         .NET Standard 2.1 — packets, DTOs, formulas. Consumed by both Game.Server and the Unity client.
├── Game.Server/       .NET 8 console app — tick loop, WebSocket host, packet handlers, persistence.
├── DebugClient/       Static HTML+JS — served by Game.Server at /debug; uses the same packet handlers as the Unity client.
└── tools/
    └── copy-dll-to-unity.ps1   Copy Game.Core build output into Unity's Assets/Plugins/.
```

## Build

```pwsh
dotnet restore MMORPG.sln
dotnet build MMORPG.sln -c Release
```

## Run

```pwsh
dotnet run --project Game.Server -c Release
```

Defaults to `http://localhost:7777`. DebugClient is at `/debug`, WebSocket endpoint at `/ws`, health at `/health`.

## Hand-off to Unity

Whenever `Game.Core` changes:

```pwsh
dotnet build Game.Core -c Release
./tools/copy-dll-to-unity.ps1
```

The script copies `Game.Core/bin/Release/netstandard2.1/*.dll` into `C:\Users\brent\UnityProjects\Necro\Assets\Plugins\`. Unity reimports automatically. The hard-coded path lives in the script and is fine for the current solo-dev setup.
