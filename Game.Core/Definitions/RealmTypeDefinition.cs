using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.realm_types — PvE / PvP / RP / RP-PvP / etc.
// (mig 0272). Normalized from realms.realm_type text in 0233.
public sealed record RealmTypeDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    int SortOrder
);
