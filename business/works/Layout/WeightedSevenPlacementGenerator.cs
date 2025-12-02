using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Generators.PlacementGenerators;
using CrossMath.Core.Types;

namespace business.works.Layout;


/// <summary>
/// 无状态权重控制生成器：精准控制最终盘面中「双7」的比例（默认 80% 单7 + 20% 双7）
/// 核心优势：纯随机 + 可配置 + 线程安全 + 零GC + 极简优雅
/// </summary>
public sealed class WeightedSevenPlacementGenerator : IPlacementGenerator
{
    /// <summary>
    /// 配置：控制最终双7比例（权重越高，越容易出现）
    /// </summary>
    /// <param name="SingleSevenWeight">保留单7的权重（默认4 → 80%）</param>
    /// <param name="DoubleSevenWeight">允许双7的权重（默认1 → 20%）</param>
    public record Config(int SingleSevenWeight = 4, int DoubleSevenWeight = 1);

    private readonly PlacementGenerator _baseGenerator = new();

    private readonly int _singleWeight;
    private readonly int _doubleWeight;
    private readonly int _totalWeight;

    public WeightedSevenPlacementGenerator(Config? config = null)
    {
        var cfg = config ?? new Config();
        _singleWeight = cfg.SingleSevenWeight;
        _doubleWeight = cfg.DoubleSevenWeight;
        _totalWeight  = _singleWeight + _doubleWeight;
    }

    public IEnumerable<Placement> Generate(ICanvas canvas)
    {
        int sevenCount = canvas.CountSevens();

        // 规则1：已有2条或以上 → 彻底封印，只允许放5
        if (sevenCount >= 2)
            return OnlyFive();

        // 规则2：恰好1条 → 命运审判！决定是否允许第二条7
        if (sevenCount == 1)
        {
            // 关键：纯随机（每次运行不同） + 线程安全 + 零分配
            bool allowDoubleSeven = Random.Shared.Next(_totalWeight) < _doubleWeight;

            return allowDoubleSeven ? GreedySeven() : OnlyFive();
        }

        // 规则3：都放
        return SevenAndFive();


        // ========== 极简神级本地方法（最干净的写法）==========
        IEnumerable<Placement> OnlyFive()
            => _baseGenerator.WithPlaceStrategies([(5, CrossType.Number)]).Generate(canvas);

        IEnumerable<Placement> GreedySeven()
            => _baseGenerator
                .WithPlaceStrategies([(7, CrossType.Number), (5, CrossType.Number)])
                .StopAtFirstMatch(true)
                .Generate(canvas);
        
        IEnumerable<Placement> SevenAndFive()
            => _baseGenerator
                .WithPlaceStrategies([(7, CrossType.Number), (5, CrossType.Number)])
                .StopAtFirstMatch(false)
                .Generate(canvas);
    }
}