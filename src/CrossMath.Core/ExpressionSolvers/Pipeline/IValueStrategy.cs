using CrossMath.Core.Expressions.Core;

namespace CrossMath.Core.ExpressionSolvers.Pipeline;

// 针对不同结构的值的枚举策略
public interface IValueStrategy
{
    IEnumerable<IExpression> FillValues(IExpression original, ExpressionSolveContext ctx);
}