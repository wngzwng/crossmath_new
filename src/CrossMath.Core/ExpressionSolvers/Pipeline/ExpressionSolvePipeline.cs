using CrossMath.Core.Expressions.Core;
namespace CrossMath.Core.ExpressionSolvers.Pipeline;

public class ExpressionSolvePipeline
{
    private readonly IOperatorStrategy _op;
    private readonly IValueStrategy _value;
    private readonly IExpressionValidator _validator;

    public ExpressionSolvePipeline(ExpressionSolveOptions options)
    {
        _op = options.OperatorStrategy!;
        _value = options.ValueStrategy!;
        _validator = options.Validator!;
    }

    public IEnumerable<IExpression> Solve(IExpression original, ExpressionSolveContext ctx)
    {
        foreach (var withOp in _op.FillOperators(original, ctx))
        {
            foreach (var solved in _value.FillValues(withOp, ctx))
            {
                if (_validator.Validate(original, solved, ctx))
                    yield return solved;
            }
        }
    }
}
