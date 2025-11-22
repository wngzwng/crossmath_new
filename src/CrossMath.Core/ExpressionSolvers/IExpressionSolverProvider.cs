using CrossMath.Core.Expressions.Schema;

namespace CrossMath.Core.ExpressionSolvers;

public interface IExpressionSolverProvider
{
    IExpressionSolver GetSolver(int expressionLength);
}