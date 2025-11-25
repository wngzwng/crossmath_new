using CrossMath.Core.Generators.CompletionCheckers;
using CrossMath.Core.Generators.ExpandControllers;
using CrossMath.Core.Generators.PlacementGenerators;
using CrossMath.Core.Generators.SearchPolicys;

namespace CrossMath.Core.Generators;

public record LayoutContext(
    IPlacementGenerator PlacementGenerator,
    ICompletionChecker CompletionChecker,
    IExpandController ExpandController,
    // ICanvasCloner Cloner,
    ISearchPolicy SearchPolicy);