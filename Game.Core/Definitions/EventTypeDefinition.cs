using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.event_types — server-side event vocabulary
// (mig 0271). Renamed from event_kinds; referenced by
// server_events.event_type (mig 0233).
public sealed record EventTypeDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    int SortOrder
);
