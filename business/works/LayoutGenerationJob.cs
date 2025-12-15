using System.Collections.Generic;
using business.works.Layout;
using CrossMath.Core.Generators;
using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Generators.Collectors;
using CrossMath.Core.Generators.CompletionCheckers;
using CrossMath.Core.Generators.ExpandControllers;
using CrossMath.Core.Generators.PlacementGenerators;
using CrossMath.Core.Generators.PlacementOrderingPolicies;
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
    public IPlacementGenerator PlacementGenerator { get; set; } = null!;

    /// <summary>初始放置点集合</summary>
    public HashSet<Placement> InitPlacements { get; init; } = new();

    /// <summary>生成目标数量（默认 100）</summary>
    public int TargetCount { get; init; } = 100;


    // --------------------- 运行配置 ---------------------

    /// <summary>停止策略（若未设置，则使用 MaxCount(TargetCount)）</summary>
    public IStopPolicy? StopPolicy { get; set; }
    
    public ICompletionChecker? CompletionChecker { get; set; }

    /// <summary>进度回调（current, total）</summary>
    public Action<int, int>? ProgressCallback { get; set; }
    
    public BucketCounter<int>? counter { get; set; }

    public IExpandController? ExpandController { get; set; }

    // --------------------- 生成执行上下文 ---------------------

    public LayoutGenContext CreateContext(HashSet<ulong>? sharedSeen = null)
    {
         var ctx =  new LayoutGenContext(
            PlacementGenerator: this.PlacementGenerator,

            CompletionChecker: this.CompletionChecker ?? DefaultCompletionChecker(),
            // new CommonCompletionChecker()
            //     .AddSizeFilter(size => size.Equals(this.CanvasSize))
            //     .AddFormulaCountFilter(cnt => 
            //         this.MinFormulaCount <= cnt && cnt <= this.MaxFormulaCount)
            //     .AddSigmaFilter(sigma => sigma <= this.MaxSigma)
            //     .AddCustomFilter(canvas =>
            //         canvas.CountEquations(exp => exp.Length == 7) is 1 or 2
            //     ),

            ExpandController: this.ExpandController ?? new ExpandComtroller(),

            // 由 Job 决定的停止策略（默认：按目标数量停止）
            StopPolicy: this.StopPolicy 
                        ?? StopPolicyFactory.MaxCount(this.TargetCount),

            // 默认 Hash / SearchPolicy / CancelToken 都由 LayoutGenContext 内部处理
            GlobalSeen: sharedSeen,
            
            PlacementOrdering: new PlacementOrderingPolicy()
        );

        if (counter != null)
        {
            counter.AllCompleted += () => ctx.Stop.RequestStop();
        }

        return ctx;
    }

    public ICompletionChecker DefaultCompletionChecker()
    {
        return new CompletionChecker().AddSizeFilter(size => size.Equals(this.CanvasSize));
    }
    
}
