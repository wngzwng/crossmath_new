using CrossMath.Core.Expressions;
using CrossMath.Core.Expressions.Core;
using CrossMath.Core.Expressions.Layout;

namespace CrossMath.Core.ExpressionSolvers.ExpressionValidators;

public class MulDivByOneLimitValidator : IExpressionValidator
{
    private readonly int _maxAllowed;

    /// <summary>
    /// 限制盘面中最多出现多少个弱算式（乘1/除1）
    /// </summary>
    public MulDivByOneLimitValidator(int maxAllowed)
    {
        _maxAllowed = maxAllowed;
    }

    public bool Validate(IExpression original, IExpression solved, ExpressionSolveContext ctx)
    {
        // 如果上下文没有 Board，无法验证，默认通过
        var board = ctx.Board;
        if (board == null)
            return true;

        // 1. 当前 solved 是否属于弱算式？
        bool currWeak = WeakExpressionUtils.ContainsMulDivByOne(solved);
        if (!currWeak)
            return true; // 当前不是弱算式 → 直接通过

        // 2. 统计当前盘面已有弱算式的数量
        int existingWeak = 0;

        foreach (var layout in ExpressionLayoutBuilder.ExtractLayouts(board.Layout, [5, 7]))
        {
            var expr = layout.ToExpression(board);
            if (!expr.IsFullyFilled)
                continue;

            if (WeakExpressionUtils.ContainsMulDivByOne(expr))
                existingWeak++;
        }

        // 3. 如果加入当前算式后数量超过上限 → 禁止
        return existingWeak + 1 <= _maxAllowed;
    }
}