using CrossMath.Core.Expressions.Core;

namespace CrossMath.Core.ExpressionSolvers;

public interface IExpressionSolver
{
    int SupportedLength { get; }
    
    IEnumerable<IExpression> Solve(IExpression expr, ExpressionSolveContext ctx);
}