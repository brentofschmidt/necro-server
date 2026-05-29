using System;

namespace Game.Core.Definitions;

// Mirrors necro_content.recipes — crafting recipes. Ingredients
// and outputs live in recipes_ingredients / recipes_outputs (mig
// 0223/0224). required_skill_level gates discovery + use.
public sealed record RecipeDefinition(
    Guid Id,
    string Slug,
    string DisplayName,
    string? Description,
    Guid? CraftingStation,
    Guid? RequiredSkillId,
    int RequiredSkillLevel,
    float CraftingTime,
    int XpReward
);
