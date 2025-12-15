using CrossMath.Core.Expressions;
using CrossMath.Core.Expressions.Core;

namespace CrossMath.Core.ExpressionSolvers.ExpressionValidators;

public class SameExpressionLimitValidator : IExpressionValidator
{
    private readonly int _maxAllowed;

    /// <summary>
    /// 限制盘面中最多出现多少个相同算式
    /// </summary>
    public SameExpressionLimitValidator(int maxAllowed)
    {
        _maxAllowed = maxAllowed;
    }
    
    public bool Validate(IExpression original, IExpression solved, ExpressionSolveContext ctx)
    {
        // 如果上下文没有 Board，无法验证，默认通过
        var board = ctx.Board;
        if (board == null)
            return true;

        var sameMap = SameExpressionCounter.WeakCounts(board);
        var curWeakId = WeakExpressionUtils.GetWeakId(solved);
        
        return sameMap.GetValueOrDefault(curWeakId, 0) + 1 <= _maxAllowed; 
    }

}