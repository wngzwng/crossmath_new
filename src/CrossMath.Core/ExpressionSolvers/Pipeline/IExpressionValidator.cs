using CrossMath.Core.Expressions.Core;
namespace CrossMath.Core.ExpressionSolvers.Pipeline;

public interface IExpressionValidator
{
    bool Validate(IExpression original, IExpression solved, ExpressionSolveContext ctx);
}
