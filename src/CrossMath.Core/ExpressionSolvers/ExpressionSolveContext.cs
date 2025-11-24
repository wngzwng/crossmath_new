using CrossMath.Core.Expressions.Core;
using CrossMath.Core.ExpressionSolvers.ExpressionValidators;
using CrossMath.Core.ExpressionSolvers.NumberPools;
using CrossMath.Core.ExpressionSolvers.OperatorPools;
using CrossMath.Core.Types;

namespace CrossMath.Core.ExpressionSolvers;

public sealed class ExpressionSolveContext
{
    public INumberPool NumPool { get; set; }
    public IOperatorPool OpPool { get; set; }

    public IExpressionValidator? Validator { get; set; }
}

/*
1. 数字池
2. 符号池


1. 验证：
单个值的验证
一堆值的验证
多重集合的验证
 */
