using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Generators.PlacementGenerators;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Generators.other
{
    // // =============================================
    // // 1. 核心上下文（策略组装中心）
    // // =============================================
    // public record LayoutContext(
    //     IPlacementGenerator PlacementGenerator,
    //     ICompletionChecker CompletionChecker,
    //     IExpandController ExpandController,
    //     ICanvasCloner Cloner,
    //     ISearchPolicy SearchPolicy);
    //
    // // =============================================
    // // 2. 核心接口定义
    // // =============================================
    // public interface ICanvasCloner
    // {
    //     ICanvas Clone(ICanvas canvas);
    // }
    //
    // public interface ICompletionChecker
    // {
    //     bool IsComplete(ICanvas canvas);
    // }
    //
    // public interface IExpandController
    // {
    //     bool ShouldExpand(ICanvas canvas, Placement? lastPlacement, int depth);
    // }
    //
    // public interface ISearchPolicy
    // {
    //     IEnumerable<BoardLayout> Search(LayoutContext ctx, ICanvas initialCanvas);
    // }

    // =============================================
    // 3. 深度优先搜索策略（带强力剪枝）
    // =============================================
    // public class DepthFirstSearchPolicy : ISearchPolicy
    // {
    //     private const int MaxDepthHardLimit = 50; // 硬防栈溢出
    //
    //     public IEnumerable<BoardLayout> Search(LayoutContext ctx, ICanvas initialCanvas)
    //     {
    //         var stack = new Stack<SearchNode>();
    //         stack.Push(new SearchNode(initialCanvas, Depth: 0, LastPlacement: null));
    //
    //         while (stack.Count > 0)
    //         {
    //             var node = stack.Pop();
    //
    //             // 完成判定：找到一个有效布局 → 立即 yield return
    //             if (ctx.CompletionChecker.IsComplete(node.Canvas))
    //             {
    //                 yield return node.Canvas.ExportBoardLayout();
    //                 // 可选：继续找更多？这里默认继续找（生成多个）
    //             }
    //
    //             // 剪枝：深度太深 or 控制器说别继续了
    //             if (node.Depth >= MaxDepthHardLimit ||
    //                 !ctx.ExpandController.ShouldExpand(node.Canvas, node.LastPlacement, node.Depth))
    //                 continue;
    //
    //             // 生成候选放置点（懒加载，只取需要的）
    //             foreach (var placement in ctx.PlacementGenerator.Generate(node.Canvas, 7, CrossType.Number))
    //             {
    //                 var cloned = ctx.Cloner.Clone(node.Canvas);
    //
    //                 if (cloned.TryApplyPlacement(placement, out var appliedPlacement))
    //                 {
    //                     stack.Push(new SearchNode(cloned, node.Depth + 1, placement));
    //                 }
    //             }
    //         }
    //     }
    //
    //     private record SearchNode(ICanvas Canvas, int Depth, Placement? LastPlacement);
    // }

    // =============================================
    // 4. 终极版 LayoutGenerator（超级简洁）
    // =============================================
    // public class LayoutGenerator
    // {
    //     public IEnumerable<BoardLayout> Generate(
    //         ICanvas initialCanvas,
    //         LayoutContext context,
    //         int maxResults = int.MaxValue)
    //     {
    //         ArgumentNullException.ThrowIfNull(initialCanvas);
    //         ArgumentNullException.ThrowIfNull(context);
    //
    //         var count = 0;
    //         foreach (var layout in context.SearchPolicy.Search(context, initialCanvas))
    //         {
    //             yield return layout;
    //             if (++count >= maxResults) yield break;
    //         }
    //     }
    // }
    /*
     public interface IPlacementGenerator
       {
           /// <summary>
           /// 根据当前画布状态，生成所有合法的候选笼子放置点
           /// 实现必须是惰性的（IEnumerable），不能一次性全部算出来
           /// </summary>
           IEnumerable<Placement> Generate(ICanvas canvas);
       }
       
     * var canvas = new CrossMathLayoutCanvas(6, 6);
       
       var context = new LayoutContext(
           PlacementGenerator: new SmartLinePlacementGenerator(minLen: 3, maxLen: 5),
           CompletionChecker:  new MinCagesCompletionChecker(minCages: 10),
           ExpandController:   new SmartExpandController(maxDepth: 25),
           Cloner:             new CanvasCloner(),
           SearchPolicy:       new DepthFirstSearchPolicy()
       );
       
       var generator = new LayoutGenerator();
       
       foreach (var layout in generator.Generate(canvas, context, maxResults: 10))
       {
           Console.WriteLine("找到一个完美布局！");
           layout.PrettyPrint();
       }
       
       LayoutGenerator
       └── 使用 LayoutContext（策略组合）
           ├── IPlacementGenerator     → 怎么生成候选放置点
           ├── ICompletionChecker      → 什么时候算“完成”
           ├── IExpandController       → 是否继续深入（剪枝核心！）
           ├── ICanvasCloner           → 高效深拷贝画布
           └── ISearchPolicy           → DFS / BFS / Beam Search / 随机等
     */
    
    
}