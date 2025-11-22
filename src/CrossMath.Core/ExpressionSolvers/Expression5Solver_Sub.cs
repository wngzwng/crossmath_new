using CrossMath.Core.Expressions.Core;
using CrossMath.Core.Types;
namespace CrossMath.Core.ExpressionSolvers;

public partial class Expression5Solver
{
    private IEnumerable<Expression5> SolveSub(Expression5 exp, ExpressionSolveContext ctx)
    {
        var iter =  SolveByOp(exp, ctx, OpType.Sub);
        foreach (var result in iter)
        {
            yield return result;
        }
    }
}