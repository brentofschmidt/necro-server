using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.target_kinds — Self / Primary / Party /
// SplashRadius etc. (mig 0232). Referenced by effects.target.
public sealed record TargetKindDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    int SortOrder
);
