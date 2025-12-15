using CrossMath.Core.Fillers;
using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Generators.SearchPolicies;

/// <summary>
/// 深度优先搜索策略 —— 经典、暴力、灵魂级去重版
/// </summary>
public sealed class DepthFirstSearchPolicy : ISearchPolicy
{
    private const int MaxDepthHardLimit = 60; // 硬防栈溢出，但比 50 更宽容
    
    public IEnumerable<BoardLayout> Search(LayoutGenContext ctx, ICanvas initialCanvas)
    {
        // 使用 ctx 提供的全局去重（自动按尺寸隔离 + 亿级容量）
        var seen = ctx.Seen;
        var stack = new Stack<SearchNode>();
        stack.Push(new SearchNode(initialCanvas.Clone(), Depth: 0, LastPlacement: null));

        while (stack.Count > 0)
        {
            
            if (ctx.CancellationToken?.IsCancellationRequested == true || ctx.Stop.IsStopping)
            {
                yield break;
            }
            
            var node = stack.Pop();

            // Step 1: 灵魂去重 —— 见过就跳过（核心！）
            ulong hash = ctx.Hash.ComputeHash(node.Canvas);
            if (!seen.Add(hash))
                continue; // 重复布局，直接丢弃，不浪费一微秒

            // Step 2: 完成判定 —— 找到一个有效布局
            if (ctx.Done.IsComplete(node.Canvas))
            {
                var layout = node.Canvas.ExportBoardLayout();

                // 可选：加入星级评定（未来接入 DifficultyRater）
                // layout.Stars = DifficultyRater.Rate(layout);

                yield return layout;
                // continue; // 找到一个就继续找下一个（生成多个) // 这里不要continue来控制逻辑
            }

            // Step 3: 剪枝 —— 深度 + 控制器双重保险
            if (node.Depth >= MaxDepthHardLimit)
                continue;

            if (!ctx.Cut.ShouldExpand(node.Canvas, node.LastPlacement, node.Depth))
                continue;


            // var placements = ctx.Gen.Generate(node.Canvas).Shuffle();
            var placements = ctx.Gen.Generate(node.Canvas);
            var orderedPlacements = ctx.Order.Order(placements, node.Canvas);
            // Step 4: 生成候选 + 放置（懒加载，最优性能）
            foreach (var placement in orderedPlacements)
            {
                var cloned = node.Canvas.Clone();

                if (cloned.TryApplyPlacement(placement, out _))
                {
                    stack.Push(new SearchNode(cloned, node.Depth + 1, placement));
                }
            }
        }
    }

    private sealed record SearchNode(
        ICanvas Canvas,
        int Depth,
        Placement? LastPlacement);
}