using CrossMath.Core.Expressions.Core;

namespace CrossMath.Core.ExpressionSolvers.SolverProviders;

public static class ExpressionSolverProviderExtensions
{
    /// <summary>
    /// 扩展方法：让 Provider 直接 Solve，而不需要手动 MatchSolver 再 Solve。
    /// </summary>
    public static IEnumerable<IExpression> Solve(
        this IExpressionSolverProvider provider,
        IExpression expr,
        ExpressionSolveContext ctx)
    {
        return provider.MatchSolver(expr.Length).Solve(expr, ctx);
    }
}