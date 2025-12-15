using CrossMath.Core.Generators.CanvasHashProvider;
using CrossMath.Core.Generators.CompletionCheckers;
using CrossMath.Core.Generators.ExpandControllers;
using CrossMath.Core.Generators.PlacementGenerators;
using CrossMath.Core.Generators.PlacementOrderingPolicies;
using CrossMath.Core.Generators.SearchPolicies;
using CrossMath.Core.Generators.StopPolicies;
using CrossMath.Core.Types;

namespace CrossMath.Core.Generators;

// public record LayoutContext(
//     IPlacementGenerator PlacementGenerator,
//     ICompletionChecker CompletionChecker,
//     IExpandController ExpandController,
//     ICanvasHashProvider Hasher,
//     ISearchPolicy SearchPolicy);
    
    
public record LayoutGenContext(
    // 停止策略（必须传入）
    IStopPolicy StopPolicy,
    
    IPlacementGenerator? PlacementGenerator = null,
    ICompletionChecker?  CompletionChecker  = null,
    IExpandController?   ExpandController   = null,
    ICanvasHashProvider? Hasher             = null,
    ISearchPolicy?       SearchPolicy       = null,
    IPlacementOrderingPolicy? PlacementOrdering = null,
    
    // 改成可空
    CancellationToken? CancellationToken = null,

    // 可选外部全局 Seen；若为 null 则内部会自动创建
    HashSet<ulong>? GlobalSeen = null)
{
    // ----------- 默认组件绑定（空值时使用 Default） -----------

    public IPlacementGenerator Gen => PlacementGenerator ?? Default.Gen;
    public ICompletionChecker  Done => CompletionChecker  ?? Default.Done;
    public IExpandController   Cut  => ExpandController   ?? Default.Cut;
    public ICanvasHashProvider Hash => Hasher             ?? Default.Hash;
    public ISearchPolicy       Go   => SearchPolicy       ?? Default.Go;
    public IPlacementOrderingPolicy Order => PlacementOrdering ?? Default.Order;
    
    public Stopper Stop { get; init; } = new();

    // ----------- 全局 Seen（支持外部注入 + Lazy 初始化） -----------

    private HashSet<ulong>? _seen;
    public HashSet<ulong> Seen => _seen ??= GlobalSeen ?? new HashSet<ulong>();
    
    public CancellationToken Token => CancellationToken ?? System.Threading.CancellationToken.None;


    // ----------- 默认策略集合（全部放在一个内部静态类中） -----------

    private static class Default
    {
        public static readonly IPlacementGenerator Gen =
            new PlacementGenerator().WithPlaceStrategies([(5, CrossType.Number)]);

        public static readonly ICompletionChecker  Done =
            new CompletionChecker();

        public static readonly IExpandController Cut =
            new ExpandComtroller();

        public static readonly ICanvasHashProvider Hash =
            ZobristCanvasHashProvider.Instance;

        public static readonly ISearchPolicy Go =
            new DepthFirstSearchPolicy();

        public static readonly IPlacementOrderingPolicy Order =
            new RandomPlacementOrderingPolicy();
    }
}
