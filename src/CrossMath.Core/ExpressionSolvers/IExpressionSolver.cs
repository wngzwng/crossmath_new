using CrossMath.Core.Expressions.Core;

namespace CrossMath.Core.ExpressionSolvers;

public interface IExpressionSolver
{
    IEnumerable<IExpression> Solve(IExpression expr, ExpressionSolveContext ctx);
}