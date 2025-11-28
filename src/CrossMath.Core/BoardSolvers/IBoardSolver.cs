using CrossMath.Core.ExpressionSolvers.SolverProviders;
using CrossMath.Core.Models;

namespace CrossMath.Core.BoardSolvers;

public interface IBoardSolver
{
    IEnumerable<BoardSolution> Solve(BoardData board, ExpressionSolverProvider  expressionSolverProvider);
}