using CrossMath.Core.Models;

namespace CrossMath.Core.ExpressionSolvers;

public static class SolveContextExtensions
{
    public static ExpressionSolveContext WithBoard(this ExpressionSolveContext ctx, BoardData board)
    {
        ctx.Board = board;
        return ctx;
    }
}
