using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.alignments — lawful/neutral/chaotic +
// good/neutral/evil grid plus the Carrion Pact special alignment
// (mig 0017).
public sealed record AlignmentDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string Description,
    int SortOrder
);
