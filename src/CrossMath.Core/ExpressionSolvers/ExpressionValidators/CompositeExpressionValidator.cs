using CrossMath.Core.Expressions.Core;

namespace CrossMath.Core.ExpressionSolvers.ExpressionValidators;

public class CompositeExpressionValidator : IExpressionValidator
{
    private readonly Func<IExpression, IExpression, ExpressionSolveContext, bool> logic;

    private CompositeExpressionValidator(
        Func<IExpression, IExpression, ExpressionSolveContext, bool> logic)
    {
        this.logic = logic;
    }

    public bool Validate(IExpression original, IExpression solved, ExpressionSolveContext ctx)
        => logic(original, solved, ctx);

    // ============ Fluent Builder API ============

    public static CompositeExpressionValidator And(params IExpressionValidator[] validators)
        => new CompositeExpressionValidator((o, s, c) =>
        {
            foreach (var v in validators)
                if (!v.Validate(o, s, c)) return false;

            return true;
        });

    public static CompositeExpressionValidator Or(params IExpressionValidator[] validators)
        => new CompositeExpressionValidator((o, s, c) =>
        {
            foreach (var v in validators)
                if (v.Validate(o, s, c)) return true;

            return false;
        });

    public static CompositeExpressionValidator Not(IExpressionValidator validator)
        => new CompositeExpressionValidator((o, s, c) => !validator.Validate(o, s, c));

    public CompositeExpressionValidator And(IExpressionValidator next)
        => And(this, next);

    public CompositeExpressionValidator Or(IExpressionValidator next)
        => Or(this, next);
}
