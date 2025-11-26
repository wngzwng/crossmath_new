using CrossMath.Core.Fillers;
using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Generators.SearchPolicys;

/// <summary>
/// 深度优先搜索策略 —— 经典、暴力、灵魂级去重版
/// </summary>
public sealed class DepthFirstSearchPolicy : ISearchPolicy
{
    private const int MaxDepthHardLimit = 60; // 硬防栈溢出，但比 50 更宽容

    // 灵魂去重专用 —— 跨实例共享，永不重复
    private static readonly HashSet<ulong> Seen = new();

    public IEnumerable<BoardLayout> Search(LayoutGenContext ctx, ICanvas initialCanvas)
    {
        // 可选：每次生成前清空去重表（生成多盘）或不清空（全局唯一）
        Seen.Clear(); // 每次重新开始

        var stack = new Stack<SearchNode>();
        stack.Push(new SearchNode(initialCanvas.Clone(), Depth: 0, LastPlacement: null));

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            // Step 1: 灵魂去重 —— 见过就跳过（核心！）
            ulong hash = ctx.Hash.ComputeHash(node.Canvas);
            if (!Seen.Add(hash))
                continue; // 重复布局，直接丢弃，不浪费一微秒

            // Step 2: 完成判定 —— 找到一个有效布局
            if (ctx.Done.IsComplete(node.Canvas))
            {
                var layout = node.Canvas.ExportBoardLayout();

                // 可选：加入星级评定（未来接入 DifficultyRater）
                // layout.Stars = DifficultyRater.Rate(layout);

                yield return layout;
                continue; // 找到一个就继续找下一个（生成多个）
            }

            // Step 3: 剪枝 —— 深度 + 控制器双重保险
            if (node.Depth >= MaxDepthHardLimit)
                continue;

            if (!ctx.Cut.ShouldExpand(node.Canvas, node.LastPlacement, node.Depth))
                continue;


            var placements = ctx.Gen.Generate(node.Canvas).Shuffle();
            // Step 4: 生成候选 + 放置（懒加载，最优性能）
            foreach (var placement in placements)
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