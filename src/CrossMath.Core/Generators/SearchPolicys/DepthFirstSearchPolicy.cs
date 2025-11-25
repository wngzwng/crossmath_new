using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Generators.SearchPolicys;

public class DepthFirstSearchPolicy : ISearchPolicy
{
    private const int MaxDepthHardLimit = 50; // 硬防栈溢出

    public IEnumerable<BoardLayout> Search(LayoutContext ctx, ICanvas initialCanvas)
    {
        var stack = new Stack<SearchNode>();
        stack.Push(new SearchNode(initialCanvas, Depth: 0, LastPlacement: null));

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            // 完成判定：找到一个有效布局 → 立即 yield return
            if (ctx.CompletionChecker.IsComplete(node.Canvas))
            {
                yield return node.Canvas.ExportBoardLayout();
                // 可选：继续找更多？这里默认继续找（生成多个）
            }

            // 剪枝：深度太深 or 控制器说别继续了
            if (node.Depth >= MaxDepthHardLimit ||
                !ctx.ExpandController.ShouldExpand(node.Canvas, node.LastPlacement, node.Depth))
                continue;

            // 生成候选放置点（懒加载，只取需要的）
            foreach (var placement in ctx.PlacementGenerator.Generate(node.Canvas))
            {
                // var cloned = ctx.Cloner.Clone(node.Canvas);
                var cloned = node.Canvas.Clone();
                

                if (cloned.TryApplyPlacement(placement, out var appliedPlacement))
                {
                    stack.Push(new SearchNode(cloned, node.Depth + 1, placement));
                }
            }
        }
    }

    private record SearchNode(ICanvas Canvas, int Depth, Placement? LastPlacement);
}