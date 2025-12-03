using business.works.Layout;
using CrossMath.Core.Generators;
using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Generators.PlacementGenerators;
using CrossMath.Core.Generators.StopPolicies;
using CrossMath.Core.Types;

namespace business.works;

public sealed class LayoutGenerationJob
{
    // --------------------- 基础配置 ---------------------

    /// <summary>画布尺寸（Size.Width × Size.Height）</summary>
    public Size CanvasSize { get; init; }

    /// <summary>算式数量范围约束（Min ≤ count ≤ Max）</summary>
    public int MinFormulaCount { get; init; }
    public int MaxFormulaCount { get; init; }

    /// <summary>允许的最大 sigma 值</summary>
    public double MaxSigma { get; init; }

    /// <summary>算式放置生成器</summary>
    public IPlacementGenerator PlacementGenerator { get; init; } = null!;

    /// <summary>初始放置点集合</summary>
    public HashSet<Placement> InitPlacements { get; init; } = new();

    /// <summary>生成目标数量（默认 100）</summary>
    public int TargetCount { get; init; } = 100;


    // --------------------- 运行配置 ---------------------

    /// <summary>停止策略（若未设置，则使用 MaxCount(TargetCount)）</summary>
    public IStopPolicy? StopPolicy { get; set; }

    /// <summary>进度回调（current, total）</summary>
    public Action<int, int>? ProgressCallback { get; set; }


    // --------------------- 生成执行上下文 ---------------------

    public LayoutGenContext CreateContext(HashSet<ulong>? sharedSeen = null)
    {
        return new LayoutGenContext(
            PlacementGenerator: this.PlacementGenerator,

            CompletionChecker: new CommonCompletionChecker()
                .AddSizeFilter(size => size.Equals(this.CanvasSize))
                .AddFormulaCountFilter(cnt => 
                    this.MinFormulaCount <= cnt && cnt <= this.MaxFormulaCount)
                .AddSigmaFilter(sigma => sigma <= this.MaxSigma)
                .AddCustomFilter(canvas =>
                    canvas.CountEquations(exp => exp.Length == 7) is 1 or 2
                ),

            ExpandController: new FormulaCountExpandController(
                new Dictionary<Size, int>
                {
                    [this.CanvasSize] = this.MaxFormulaCount
                }
            ),

            // 由 Job 决定的停止策略（默认：按目标数量停止）
            StopPolicy: this.StopPolicy 
                        ?? StopPolicyFactory.MaxCount(this.TargetCount),

            // 默认 Hash / SearchPolicy / CancelToken 都由 LayoutGenContext 内部处理
            GlobalSeen: sharedSeen
        );
    }
}
