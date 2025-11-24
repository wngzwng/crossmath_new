using CrossMath.Core.Expressions.Core;

namespace CrossMath.Core.ExpressionSolvers.ExpressionValidators;

public class ExpressionValidator : IExpressionValidator
{
    public bool Validate(IExpression original, IExpression solvedExp)
    {
        return true;
    }
}