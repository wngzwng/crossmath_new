using CrossMath.Core.Types;

namespace CrossMath.Core.Models;

public static class BoardDataSolveExtensions
{
    public static bool SolverFill(this BoardData board, RowCol pos, string val)
    {
        return board.SetValueOnly(pos, val);
    }

    public static bool SolverClear(this BoardData board, RowCol pos)
    {
        return board.ClearValueOnly(pos);
    }
}