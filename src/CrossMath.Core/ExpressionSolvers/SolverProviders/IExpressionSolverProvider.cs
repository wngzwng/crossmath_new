using CrossMath.Core.Expressions.Schema;

namespace CrossMath.Core.ExpressionSolvers.SolverProviders;

public interface IExpressionSolverProvider
{
    IExpressionSolver MatchSolver(int expressionLength);
}