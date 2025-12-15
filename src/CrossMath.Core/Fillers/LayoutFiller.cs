
using System.Text;
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
    
    private static readonly Lock Lock = new();

    private readonly IExpressionSolverProvider _solverProvider;

    private ExpressionSolveContext _ctx = null!;
    private int _solutionSampleLimit = DefaultSolutionSampleLimit;
    private FirstFillSelectMode _firstFillMode = FirstFillSelectMode.First;

    // 图结构
    private Dictionary<string, ExpressionLayout> _exprMap = null!;
    private Dictionary<string, HashSet<string>> _intersectionGraph = null!;
    private string _startExpressionId = null!;
    private List<ExpressionLayout> _expressionLayouts = null!;
    private List<string> ids = null!;

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

    public void Reset()
    {
        _startExpressionId = null;
        _exprMap = null;
        _intersectionGraph = null;
        _expressionLayouts = null;
        ids = null;
        _firstFillMode = FirstFillSelectMode.First;
        _solutionSampleLimit = DefaultSolutionSampleLimit;
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
        Reset();
        if (!BuildGraph(layout, allowedLengths))
        {
            resultBoard = null;
            successAttempt = null;
            return false;
        }

        // if (!ValidateGraph())
        // {
        //     Console.WriteLine("[Error] Graph is not valid.");
        //     layout.LogicPrettyPrint();
        // }

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
                if (!_exprMap.TryGetValue(exprId, out var expr))
                {
                    PrintGraph(board.Layout);
                }
                var layout = _exprMap[exprId];
                var expression = layout.ToExpression(board);

                // 已满足，直接继续传播
                if (expression.Evaluate())
                    return true;

                // 采样若干解，随机选一个
                var randomSolution = _solverProvider
                    .Solve(expression, _ctx.WithBoard(board))
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
        // _expressionLayouts  = layouts.ToList();
        // ids = new List<string>();
        // // _exprMap = _expressionLayouts.ToDictionary(x =>
        // // {
        // //     ids.Add(x.Id.Value);
        // //     return x.Id.Value;
        // // });
        // _exprMap = new Dictionary<string, ExpressionLayout>();
        // lock (Lock)
        // {
        //     for (int i = 0; i < layouts.Count; i++)
        //     {
        //         var id = layouts[i].Id.Value;
        //         if (id == null)
        //             Console.WriteLine($"[{i}] ID is NULL!");
        //
        //         Console.WriteLine($"[{i}] {id} (hash={id?.GetHashCode()}) Length={id?.Length}");
        //         var bytes = Encoding.UTF8.GetBytes(id);
        //         Console.WriteLine(
        //             $"[{i}] '{id}' len={id.Length} bytes={BitConverter.ToString(bytes)} "
        //             + $"control={(bytes.Any(b => b < 32) ? "YES" : "NO")}"
        //         );
        //         ids.Add(id);
        //         _exprMap.Add(id, layouts[i]);
        //     }
        // }

        _exprMap = layouts.ToDictionary(exprLayout => exprLayout.Id.Value);
        _intersectionGraph = ExpressionLayoutGraphUtils.BuildIntersectionGraph(layouts);
        
        _startExpressionId = _firstFillMode switch
        {
            FirstFillSelectMode.First   => layouts[0].Id.Value,
            FirstFillSelectMode.Random  => layouts.RandomElementByShuffle().Id.Value,
            _ => throw new ArgumentOutOfRangeException(nameof(_firstFillMode))
        };

        // _exprMap = layouts.ToDictionary(x => x.Id.Value);
        // _intersectionGraph = ExpressionLayoutGraphUtils.BuildIntersectionGraph(layouts);
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

    private bool ValidateGraph()
    {
        var isValid = true;
        var exprKeys = new HashSet<string>(_exprMap.Keys);

        Console.WriteLine("\n=== Graph Validation ===");

        // 1. 检查 _intersectionGraph 自己的 key
        foreach (var key in _intersectionGraph.Keys)
        {
            if (!exprKeys.Contains(key))
            {
                Console.WriteLine($"[ERROR] Graph key '{key}' not in _exprMap");
                isValid = false;
            }
        }

        // 2. 检查邻居列表
        foreach (var (key, neighbors) in _intersectionGraph)
        {
            foreach (var nb in neighbors)
            {
                if (!exprKeys.Contains(nb))
                {
                    Console.WriteLine($"[ERROR] Neighbor '{nb}' referenced by '{key}' not in _exprMap");
                    isValid = false;
                }
            }
        }
        Console.WriteLine("Validation done.");
        return isValid;
    }


    private void PrintGraph(BoardLayout layout)
    {
        Console.WriteLine("===== LayoutFiller Graph Info =====");
        layout.LogicPrettyPrint();
        // Start node
        Console.WriteLine($"\nStart Expression = {_startExpressionId}");

        // Expression map
        Console.WriteLine("\n=== Expression Map ===");
        foreach (var (id, explayout) in _exprMap)
        {
            Console.WriteLine($"ID: {id}: {explayout}");
        }

        // Intersection graph
        Console.WriteLine("\n=== Intersection Graph ===");
        foreach (var (id, neighbors) in _intersectionGraph)
        {
            Console.WriteLine($"{id} → {string.Join(", ", neighbors)}");
        }

        // Validation
        ValidateGraph();
    }

}
