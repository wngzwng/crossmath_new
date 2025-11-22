// using CrossMath.Core.Expressions.Core;
// using CrossMath.Core.Expressions.Layout;
// using CrossMath.Core.Types;
// using CrossMath.Core.Models;
// using CrossMath.Core.ExpressionSolvers;
// using CrossMath.Core.Codec;
// namespace CrossMath.Core.Fillers;
//
// /// <summary>
// /// 棋盘填充器
// /// </summary>
// public class LayoutFiller
// {
//     /* 尝试次数 tryCount
//      * 1. 提取盘面的表达式
//      * 2. 构建算式本身的关系图，以及获取首个算式的ID
//      * 3. 通过这个图 -> 下一个算式
//      * 4. 核心动作 => 求解，填数，通知
//      * 4.1 求解 => IExpressionSlover
//      * 4.2 空格位置 -> 提取数。=> (空格位置， 候选数）
//      * 4.3 填空 -> 盘面填空， 空格 -> 算式的映射关系， 空格->算式，算式通知
//      */
//     
//     private readonly IExpressionSolver _solver;
//     private ExpressionSolveContext ctx { get; set; }
//     private int tryCount = 100;
//
//     private Dictionary<string, HashSet<string>> exprIntersectionGraph = new();
//     private Dictionary<string, HashSet<RowCol>> exprToPosMap = new();
//     private Dictionary<string, ExpressionLayout> exprMap = new();
//     private string startExpressionID = null;
//     
//     public LayoutFiller(IExpressionSolver solver)
//     {
//         _solver = solver;
//     }
//
//     public void Setup()
//     {
//         ctx = new ExpressionSolveContext()
//         {
//             NumPool = new NumberPool(1, 20),
//             OpPool = new OperatorPool()
//         };
//     }
//
//     public bool TryFill(BoardLayout layout, int tryCount, out BoardData? boardData, out int? successIndex)
//     {
//         Setup();
//         boardData = null;
//         successIndex = null;
//     
//         // ✅ C# 11 及之前版本使用标准集合初始化
//         Build(layout, new List<int> { 5 });
//         var orginCount = tryCount;
//         while (tryCount-- > 0)
//         {
//             var emptyBoard = BoardDataCodec.Decode(layout);
//         
//             if (TryFillOnce(emptyBoard))
//             {
//                 boardData = emptyBoard;
//                 successIndex = orginCount - tryCount;
//                 return true;
//             }
//         }
//
//         return false;
//     }
//
//     public bool TryFillOnce(BoardData boardData)
//     {
//         return BreadthFirstTraversal<string>(
//             startExpressionID,
//             expressionId => exprIntersectionGraph.TryGetValue(expressionId, out var neighbors) 
//                 ? neighbors 
//                 : Enumerable.Empty<string>(),
//             (expressionId) =>
//             {
//                 var expression = GetExpression(exprMap[expressionId], boardData);
//
//                 // 已经求解完成的表达式，继续遍历
//                 if (expression.Evaluate())
//                     return true;
//
//                 var solutions = _solver.Solve(expression, ctx);
//                 if (solutions.Any())
//                 {
//                     
//                 }
//                 // 求解阶段
//                 if (_solver.Solve(expression) is { } solvedExpression)
//                 {
//                     FillBoard(exprMap[expressionId], boardData, solvedExpression.GetTokens());
//                     return true; // ✅ 求解成功，继续
//                 }
//                 
//                 return false; // ✅ 中止 BFS
//             });
//     }
//
//     public IExpression GetExpression(ExpressionLayout expressionLayout, BoardData boardData)
//     {
//         var tokens = new List<string?>(expressionLayout.Schema.TokenCount);
//         foreach (var pos in expressionLayout.Coordinates)
//         {
//             tokens.Add(boardData.TryGetValue(pos, out var val) ? val : String.Empty);
//         }
//         return BaseExpression.Create(tokens);
//     }
//
//     public void FillBoard(ExpressionLayout expressionLayout, BoardData boardData, List<string> tokens)
//     {
//         var index = 0;
//         foreach (var pos in expressionLayout.Coordinates)
//         {
//             var value = tokens[index++];
//             boardData.TrySetValue(pos, value);
//         }
//     }
//
//     private void Build(BoardLayout layout, List<int> allowExpressionLengths)
//     {
//         var expressionLayouts = ExpressionLayoutBuilder.ExtractLayouts(layout, allowExpressionLengths);
//         startExpressionID = expressionLayouts[0].ExpressionID;
//         
//         exprIntersectionGraph = ExpressionLayoutBuilder.BuildIntersectionGraph(expressionLayouts);
//         exprToPosMap = ExpressionLayoutBuilder.BuildExprToPosMap(expressionLayouts);
//         exprMap = expressionLayouts.ToDictionary(x => x.ExpressionID, x => x);
//     }
//     
//     /// <summary>
//     /// 广度优先遍历（完全泛型）
//     /// </summary>
//     /// <typeparam name="T">节点类型</typeparam>
//     /// <param name="root">起始节点</param>
//     /// <param name="getNeighbors">邻居节点获取函数</param>
//     /// <param name="callback">
//     /// 访问节点的回调函数。
//     /// 返回 <c>false</c> 表示希望中止遍历；
//     /// 返回 <c>true</c> 表示继续。
//     /// </param>
//     /// <returns>
//     /// <c>true</c> 表示遍历完整结束；
//     /// <c>false</c> 表示在回调中被中途终止。
//     /// </returns>
//     public static bool BreadthFirstTraversal<T>(
//         T root,
//         Func<T, IEnumerable<T>> getNeighbors,
//         Func<T, bool> callback)
//     {
//         if (root == null)
//             throw new ArgumentNullException(nameof(root), "起始节点不能为空。");
//         if (getNeighbors == null)
//             throw new ArgumentNullException(nameof(getNeighbors));
//         if (callback == null)
//             throw new ArgumentNullException(nameof(callback));
//
//         var visited = new HashSet<T>();
//         var queue = new Queue<T>();
//
//         visited.Add(root);
//         queue.Enqueue(root);
//
//         while (queue.Count > 0)
//         {
//             var current = queue.Dequeue();
//
//             // 调用回调函数处理节点
//             if (!callback(current))
//             {
//                 // ✅ 回调要求终止遍历
//                 return false;
//             }
//
//             // 获取邻居节点（防御式编程）
//             IEnumerable<T>? neighbors;
//             try
//             {
//                 neighbors = getNeighbors(current);
//             }
//             catch (Exception ex)
//             {
//                 System.Diagnostics.Debug.WriteLine($"[BFS] 获取邻居节点时异常: {ex.Message}");
//                 continue;
//             }
//
//             if (neighbors == null) continue;
//
//             foreach (var neighbor in neighbors)
//             {
//                 // ✅ HashSet.Add 返回 true 表示首次访问
//                 if (visited.Add(neighbor))
//                 {
//                     queue.Enqueue(neighbor);
//                 }
//             }
//         }
//
//         return true; // ✅ 遍历完整完成
//     }
// }