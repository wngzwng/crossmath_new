using CrossMath.Core.BoardSolvers;
using CrossMath.Core.ExpressionSolvers.SolverProviders;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.HoleDiggers.HoleVadidators;

public class DefaultHoleValidator : IHoleValidator
{
    private IBoardSolver _boardSolver;

    public DefaultHoleValidator(IBoardSolver boardSolver)
    {
        _boardSolver = boardSolver;
    }

    public static DefaultHoleValidator Create(IBoardSolver? boardSolver = null)
    {
        boardSolver = boardSolver ?? new BoardSolver();
        return new DefaultHoleValidator(boardSolver);
    }
    public bool IsValidHollowOut(BoardData board, RowCol coord)
    {   
        var workingBoard = board.Clone();
        if (!workingBoard.ExtractAnswer(coord, out var answer))
        {
            return false;
        }

        var solutions = _boardSolver.Solve(workingBoard, ExpressionSolverProvider.CreateDefault());
        if (solutions.Take(2).Count() != 1) // 是否唯一解
        {
            return false;
        }
        
        return true;
    }
}