using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.reputation_standings — hated / hostile /
// unfriendly / neutral / friendly / honored / revered / exalted
// (mig 0255). min_value / max_value encode the value-range CASE
// that used to live in a generated column on reputation.
public sealed record ReputationStandingDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    int? MinValue,
    int? MaxValue,
    int SortOrder
);
