using System.Collections.Generic;
using business.works.Layout;
using CrossMath.Core.Generators;
using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Models;
using CrossMath.Core.Types;
namespace business.works;

/// <summary>
/// CrossMath 布局生成运行器（统一入口）
/// 对外永远只有一个方法：Run
/// 内部自动选择当前最强策略（2025+ 推荐 Fountain）
/// </summary>
public sealed class LayoutGenerationJobRunner
{
    private readonly LayoutGeneratorSimple _generator = new();
    // private readonly HashSet<ulong> _globalSeen = new(1_000_000);

    /// <summary>
    /// 统一生成入口（对外唯一公共方法）
    /// 始终使用当前最优策略生成布局，性能、多样性、内存友好拉满
    /// </summary>
    public IEnumerable<BoardLayout> Run(LayoutGenerationJob job) 
        => GenerateFountain(job);

    // ──────────────────────────────────────────────────────────────
    // 当前最强实现：多起点喷泉生成器（推荐）
    // ──────────────────────────────────────────────────────────────
    private IEnumerable<BoardLayout> GenerateFountain(LayoutGenerationJob job)
    {
        HashSet<ulong> sharedSeen = new();
        var ctx = job.CreateContext(sharedSeen);

        if (!job.InitPlacements.Any())
            yield break;

        var streams = CreateLazyStreams(job, ctx)
            .Where(s => s.Enumerator != null)
            .ToList();

        if (streams.Count == 0)
            yield break;

        int yielded = 0;
        int target = job.TargetCount;

        // while (streams.Count > 0 && yielded < target)
        while (streams.Count > 0)
        {
            for (int i = streams.Count - 1; i >= 0; i--)
            {
                var stream = streams[i];

                if (stream.Enumerator.MoveNext())
                {
                    yield return stream.Enumerator.Current;

                    yielded++;
                    job.ProgressCallback?.Invoke(yielded, target);

                    // if (yielded >= target)
                    //     yield break;
                }
                else
                {
                    stream.Enumerator.Dispose();
                    streams.RemoveAt(i);  // 删除 i，后面的元素前移，但我们下次访问 i-1，不受影响
                }
            }
        }
    }

    // ──────────────────────────────────────────────────────────────
    // 兼容旧代码（可选保留，建议标记 Obsolete）
    // ──────────────────────────────────────────────────────────────
    public IEnumerable<BoardLayout> GenerateSequential(LayoutGenerationJob job)
    {
        HashSet<ulong> sharedSeen = new(1_000_000);
        var ctx = job.CreateContext(sharedSeen);

        foreach (var canvas in BuildInitCanvasGroup(job.CanvasSize, job.InitPlacements))
        {
            foreach (var layout in _generator.Generate(canvas, ctx))
            {
                job.ProgressCallback?.Invoke(System.Threading.Interlocked.Increment(ref _dummyCounter), job.TargetCount);
                yield return layout;
            }
        }
    }

    // ──────────────────────────────────────────────────────────────
    // 私有辅助方法
    // ──────────────────────────────────────────────────────────────
    private IEnumerable<(ICanvas InitialCanvas, IEnumerator<BoardLayout> Enumerator)> CreateLazyStreams(
        LayoutGenerationJob job, LayoutGenContext ctx)
    {
        foreach (var canvas in BuildInitCanvasGroup(job.CanvasSize, job.InitPlacements))
        {
            yield return (canvas, _generator.Generate(canvas, ctx).GetEnumerator());
        }
    }

    private static IEnumerable<ICanvas> BuildInitCanvasGroup(Size size, IEnumerable<Placement> placements)
    {
        foreach (var p in placements)
        {
            var canvas = new LayoutCanvas(size);
            if (canvas.TryApplyPlacement(p, out _))
                yield return canvas;
        }
    }

    // 用于兼容旧进度回调的临时计数器（仅在旧方法中使用）
    private int _dummyCounter = 0;
}
