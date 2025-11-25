
using CrossMath.Core.Models;
using CrossMath.Core.Codec;
using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.ExpressionSolvers;
using CrossMath.Core.ExpressionSolvers.SolverProviders;

namespace CrossMath.Core.Fillers;

/// <summary>
/// 棋盘填充器（Expression Layout → Graph → 求解 → 填盘）
/// </summary>
public class LayoutFiller
{
    // ------------------------------------------------------------
    // 构造 / 基础配置
    // ------------------------------------------------------------

    private static readonly List<int> s_defaultExpressionLengths = new() { 5, 7 };
    private readonly IExpressionSolverProvider _solverProvider;

    private ExpressionSolveContext ctx { get; set; }
    private int tryCount = 100;
    private int solutionSampleLimit = 10;

    private FirstFillSelectMode firstFillMode = FirstFillSelectMode.First;
    // 图结构 & 映射
    private Dictionary<string, HashSet<string>> exprIntersectionGraph = new();
    private Dictionary<string, ExpressionLayout> exprMap = new();
    private string startExpressionID = null;

    public LayoutFiller(IExpressionSolverProvider provider)
    {
        _solverProvider = provider;
    }

    /// <summary>设置求解上下文</summary>
    public void Setup(ExpressionSolveContext ctx)
    {
        this.ctx = ctx;
    }
    
    public void SetSolutionSampleLimit(int limit)
    {
        solutionSampleLimit = Math.Max(1, limit);
    }

    public void SetFirstFillSelectMode(FirstFillSelectMode mode)
    {
        firstFillMode = mode;
    }


    // ------------------------------------------------------------
    // 对外主流程：多次尝试填盘
    // ------------------------------------------------------------

    public bool TryFill(
        BoardLayout layout,
        int tryCount,
        out BoardData? boardData,
        out int? successIndex) => TryFill(layout, tryCount, null, out boardData, out successIndex);
    public bool TryFill(
        BoardLayout layout,
        int tryCount,
        List<int>? supportExpressionLengths,
        out BoardData? boardData,
        out int? successIndex)
    {
        supportExpressionLengths ??= s_defaultExpressionLengths;

        boardData = null;
        successIndex = null;

        Build(layout, supportExpressionLengths);

        var originCount = tryCount;

        while (tryCount-- > 0)
        {
            var emptyBoard = BoardDataCodec.Decode(layout);

            if (TryFillOnce(emptyBoard))
            {
                boardData = emptyBoard;
                successIndex = originCount - tryCount;
                return true;
            }
        }

        return false;
    }

    // ------------------------------------------------------------
    // 单次填盘流程（核心 BFS 填充）
    // ------------------------------------------------------------

    public bool TryFillOnce(BoardData boardData)
    {
        return BreadthFirstTraversal<string>(
            startExpressionID,
            id => exprIntersectionGraph.TryGetValue(id, out var neighbors)
                    ? neighbors
                    : Enumerable.Empty<string>(),

            expressionId =>
            {
                var expression = exprMap[expressionId].ToExpression(boardData);

                // 已求解完成 → 继续 BFS
                if (expression.Evaluate())
                    return true;

                // 求解表达式，并从前 N 个解随机选一个
                var randomSolution = _solverProvider
                    .Solve(expression, ctx)
                    .Take(solutionSampleLimit)
                    .RandomElementByReservoirSampling();
                
                if (randomSolution is null)
                {
                    // 无解时终止 BFS
                    return false;
                }

                FillBoard(exprMap[expressionId], boardData, randomSolution.GetTokens());
                return true;
            });
    }

    // ------------------------------------------------------------
    // 填盘（将求解 token 写回 BoardData）
    // ------------------------------------------------------------

    private void FillBoard(ExpressionLayout expressionLayout, BoardData boardData, List<string> tokens)
    {
        var index = 0;
        foreach (var pos in expressionLayout.Cells)
        {
            boardData.SetValueOnly(pos, tokens[index++]);
        }
    }

    // ------------------------------------------------------------
    // 构建 Expression Graph & 映射表
    // ------------------------------------------------------------

    private void Build(BoardLayout layout, List<int> allowExpressionLengths)
    {
        var expressionLayouts = ExpressionLayoutBuilder.ExtractLayouts(layout, allowExpressionLengths);

        startExpressionID = firstFillMode switch
        {
            FirstFillSelectMode.First =>  expressionLayouts[0].Id.Value,
            FirstFillSelectMode.Random =>  expressionLayouts.RandomElementByShuffle().Id.Value,
            _ => throw new ArgumentOutOfRangeException(nameof(firstFillMode), firstFillMode, null)
        };
      
        exprMap = expressionLayouts.ToDictionary(x => x.Id.Value, x => x);
        exprIntersectionGraph = ExpressionLayoutGraphUtils.BuildIntersectionGraph(expressionLayouts);
    }

    // ------------------------------------------------------------
    // 通用 BFS 工具方法
    // ------------------------------------------------------------

    /// <summary>
    /// 广度优先遍历（可中断）
    /// </summary>
    public static bool BreadthFirstTraversal<T>(
        T root,
        Func<T, IEnumerable<T>> getNeighbors,
        Func<T, bool> callback)
    {
        if (root == null)
            throw new ArgumentNullException(nameof(root));
        if (getNeighbors == null)
            throw new ArgumentNullException(nameof(getNeighbors));
        if (callback == null)
            throw new ArgumentNullException(nameof(callback));

        var visited = new HashSet<T>();
        var queue = new Queue<T>();

        visited.Add(root);
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            // 回调返回 false → 中断遍历
            if (!callback(current))
                return false;

            IEnumerable<T>? neighbors;

            try
            {
                neighbors = getNeighbors(current);
            }
            catch
            {
                continue;
            }

            if (neighbors == null)
                continue;

            foreach (var neighbor in neighbors)
            {
                if (visited.Add(neighbor))
                    queue.Enqueue(neighbor);
            }
        }

        return true;
    }
}
