using CrossMath.Core.Expressions.Core;

namespace CrossMath.Core.ExpressionSolvers.ExpressionValidators;

public interface IExpressionValidator
{
    bool Validate(IExpression original, IExpression solvedExp);
}