using business.works.Layout;
using CrossMath.Core.Generators.PlacementGenerators;
using CrossMath.Core.Types;

namespace business.works;

public sealed class LayoutGenerationJob
{
    public Size CanvasSize { get; init; }

    public int MinFormulaCount { get; init; }
    public int MaxFormulaCount { get; init; }

    public double MaxSigma { get; init; }

    public IPlacementGenerator PlacementGenerator { get; init; }
    
    public HashSet<Placement> InitPlacements{ get; init; }

    public int TargetCount { get; init; } = 100;

    public Action<int, int>? ProgressCallback { get; set; }
}
