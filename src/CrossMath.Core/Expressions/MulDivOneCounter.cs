using CrossMath.Core.Expressions;
using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Models;

namespace CrossMath.Core.Expressions;

public static class MulDivOneCounter
{
    public static int Count(BoardData board)
    {
        var existingWeak = 0;
        foreach (var layout in ExpressionLayoutBuilder.ExtractLayouts(board.Layout, [5, 7]))
        {
            var expr = layout.ToExpression(board);
            if (!expr.IsFullyFilled)
                continue;

            if (WeakExpressionUtils.ContainsMulDivByOne(expr))
                existingWeak++;
        }
        
        return existingWeak;
    }
}