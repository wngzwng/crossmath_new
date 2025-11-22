using CrossMath.Core.Expressions.Core;
using CrossMath.Core.Types;

namespace CrossMath.Core.ExpressionSolvers;

public sealed class ExpressionSolveContext
{
    public ValidationMode Validation { get; set; } = ValidationMode.Partial;
    public Func<IExpression, IExpression, bool>? CustomValidator { get; set; }
    public NumberPool NumPool { get; set; }
    public OperatorPool OpPool { get; set; }
    
    public bool IsInRange(int v) => NumPool.Contains(v); // 加 Min/Max 如果需要
}

/*
1. 数字池
2. 符号池


1. 验证：
单个值的验证
一堆值的验证
多重集合的验证
 */
