
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
    private const int DefaultSolutionSampleLimit = 10;
    private const int DefaultTryCount = 100;

    private readonly IExpressionSolverProvider _solverProvider;

    private ExpressionSolveContext _ctx = null!;
    private int _solutionSampleLimit = DefaultSolutionSampleLimit;
    private FirstFillSelectMode _firstFillMode = FirstFillSelectMode.First;

    // 图结构
    private Dictionary<string, ExpressionLayout> _exprMap = null!;
    private Dictionary<string, HashSet<string>> _intersectionGraph = null!;
    private string _startExpressionId = null!;

    public LayoutFiller(IExpressionSolverProvider provider)
    {
        _solverProvider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public LayoutFiller Setup(ExpressionSolveContext ctx)
    {
        _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
        return this;
    }

    public LayoutFiller WithSolutionSampleLimit(int limit)
    {
        _solutionSampleLimit = Math.Max(1, limit);
        return this;
    }

    public LayoutFiller WithFirstFillMode(FirstFillSelectMode mode)
    {
        _firstFillMode = mode;
        return this;
    }

    // ==================== 主入口 ====================

    public bool TryFill(
        BoardLayout layout,
        int maxAttempts,
        List<int>? allowedLengths,
        out BoardData? resultBoard,
        out int? successAttempt)
    {
        allowedLengths ??= new() { 5, 7 };

        if (!BuildGraph(layout, allowedLengths))
        {
            resultBoard = null;
            successAttempt = null;
            return false;
        }

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            var board = BoardDataCodec.Decode(layout);

            if (TryFillOnce(board))
            {
                resultBoard = board;
                successAttempt = attempt;
                return true;
            }
        }

        resultBoard = null;
        successAttempt = null;
        return false;
    }

    // ==================== 单次填充（核心） ====================

    private bool TryFillOnce(BoardData board)
    {
        return BreadthFirstFill(
            start: _startExpressionId,
            getNeighbors: id => _intersectionGraph.GetValueOrDefault(id) ?? Enumerable.Empty<string>(),
            tryProcessNode: exprId =>
            {
                var layout = _exprMap[exprId];
                var expression = layout.ToExpression(board);

                // 已满足，直接继续传播
                if (expression.Evaluate())
                    return true;

                // 采样若干解，随机选一个
                var randomSolution = _solverProvider
                    .Solve(expression, _ctx)
                    .Take(_solutionSampleLimit)
                    .RandomElementByReservoirSampling();

                if (randomSolution == null)
                    return false; // 无解，失败

                FillExpression(layout, board, randomSolution.GetTokens());
                return true;
            });
    }

    private static void FillExpression(ExpressionLayout layout, BoardData board, IReadOnlyList<string> tokens)
    {
        if (tokens.Count != layout.Cells.Count)
            throw new InvalidOperationException($"Token count mismatch: expected {layout.Cells.Count}, got {tokens.Count}");

        for (int i = 0; i < layout.Cells.Count; i++)
        {
            board.SetValueOnly(layout.Cells[i], tokens[i]);
        }
    }

    // ==================== 图构建 ====================

    private bool BuildGraph(BoardLayout layout, List<int> allowedLengths)
    {
        var layouts = ExpressionLayoutBuilder.ExtractLayouts(layout, allowedLengths);

        if (layouts.Count == 0)
            return false;

        _startExpressionId = _firstFillMode switch
        {
            FirstFillSelectMode.First   => layouts[0].Id.Value,
            FirstFillSelectMode.Random  => layouts.RandomElementByShuffle().Id.Value,
            _ => throw new ArgumentOutOfRangeException(nameof(_firstFillMode))
        };

        _exprMap = layouts.ToDictionary(x => x.Id.Value);
        _intersectionGraph = ExpressionLayoutGraphUtils.BuildIntersectionGraph(layouts);
        return true;
    }

    // ==================== 更清晰语义的 BFS ====================

    private static bool BreadthFirstFill<T>(
        T start,
        Func<T, IEnumerable<T>> getNeighbors,
        Func<T, bool> tryProcessNode)
    {
        var visited = new HashSet<T>();
        var queue = new Queue<T>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (!tryProcessNode(current))
                return false; // 某节点无法处理 → 整盘失败

            foreach (var neighbor in getNeighbors(current))
            {
                if (visited.Add(neighbor))
                    queue.Enqueue(neighbor);
            }
        }

        return true; // 所有表达式都处理成功
    }
}
