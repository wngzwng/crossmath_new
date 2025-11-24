using CrossMath.Core.Expressions.Core;

namespace CrossMath.Core.ExpressionSolvers.Pipeline;

// 针对不同结构的运算符填补策略
public interface IOperatorStrategy
{
    IEnumerable<IExpression> FillOperators(IExpression original, ExpressionSolveContext ctx);
}