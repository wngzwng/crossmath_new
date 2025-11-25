using CrossMath.Core.ExpressionSolvers.Expression5Solvers;
using CrossMath.Core.ExpressionSolvers.Expression7Solvers;

namespace CrossMath.Core.ExpressionSolvers.SolverProviders;
public sealed class ExpressionSolverProvider : IExpressionSolverProvider
{
    private readonly Dictionary<int, IExpressionSolver> _solvers;

    public ExpressionSolverProvider(params IExpressionSolver[] solvers)
    {
        _solvers = solvers.ToDictionary(s => s.SupportedLength);
    }

    public IExpressionSolver MatchSolver(int expressionLength)
    {
        if (_solvers.TryGetValue(expressionLength, out var solver))
            return solver;

        throw new NotSupportedException($"Unsupported expression length: {expressionLength}");
    }
    
    /// <summary>
    /// 创建支持任意 solver 的 Provider
    /// 要求每个 solver 必须能告诉自身适配的长度
    /// </summary>
    public static ExpressionSolverProvider Create(params IExpressionSolver[] solvers)
    {
        return new ExpressionSolverProvider(solvers);
    }

    // -------------------------------------------------------
    // 静态工厂：默认创建支持 5 和 7 的 Provider（推荐）
    // -------------------------------------------------------
    public static ExpressionSolverProvider CreateDefault()
    {
        return new ExpressionSolverProvider(new Expression5Solver(), new Expression7Solver());
    }

    // -------------------------------------------------------
    // 静态工厂：用户传入数字 → 自动创建相应 Solver
    // -------------------------------------------------------
    public static ExpressionSolverProvider CreateDefault(params int[] lengths)
    {
        var dict = new Dictionary<int, IExpressionSolver>();

        foreach (var len in lengths.Distinct())
        {
            dict[len] = len switch
            {
                5 => new Expression5Solver(),
                7 => new Expression7Solver(),

                // 未来扩展时，只需加：
                // 9 => new Expression9Solver(),
                // 11 => new Expression11Solver(),

                _ => throw new NotSupportedException($"No default solver for expression length {len}")
            };
        }

        return new ExpressionSolverProvider(dict.Values.ToArray());
    }
}
