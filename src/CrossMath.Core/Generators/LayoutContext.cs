using CrossMath.Core.Generators.CanvasHashProvider;
using CrossMath.Core.Generators.CompletionCheckers;
using CrossMath.Core.Generators.ExpandControllers;
using CrossMath.Core.Generators.PlacementGenerators;
using CrossMath.Core.Generators.SearchPolicys;
using CrossMath.Core.Types;

namespace CrossMath.Core.Generators;

// public record LayoutContext(
//     IPlacementGenerator PlacementGenerator,
//     ICompletionChecker CompletionChecker,
//     IExpandController ExpandController,
//     ICanvasHashProvider Hasher,
//     ISearchPolicy SearchPolicy);
    
    
public record LayoutGenContext(
    IPlacementGenerator? PlacementGenerator = null,
    ICompletionChecker?  CompletionChecker  = null,
    IExpandController?  ExpandController  = null,
    ICanvasHashProvider? Hasher           = null,
    ISearchPolicy?       SearchPolicy     = null)
{
    public IPlacementGenerator Gen  => PlacementGenerator ?? Default.Gen;
    public ICompletionChecker  Done => CompletionChecker  ?? Default.Done;
    public IExpandController   Cut  => ExpandController   ?? Default.Cut;
    public ICanvasHashProvider  Hash => Hasher             ?? Default.Hash;
    public ISearchPolicy       Go   => SearchPolicy       ?? Default.Go;

    private static class Default
    {
        public static readonly IPlacementGenerator Gen =
            new PlacementGenerator().WithPlaceStrategies([(5, CrossType.Number)]);
        public static readonly ICompletionChecker  Done = new CompletionChecker();
        public static readonly IExpandController   Cut  = new ExpandComtroller();
        public static readonly ICanvasHashProvider Hash = ZobristCanvasHashProvider.Instance;
        public static readonly ISearchPolicy       Go   = new DepthFirstSearchPolicy();
    }
}