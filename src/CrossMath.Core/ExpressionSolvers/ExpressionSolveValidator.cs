using CrossMath.Core.Expressions.Core;
namespace CrossMath.Core.ExpressionSolvers;

public static class ExpressionValidator
{
    public static bool IsValid(this ExpressionSolveContext ctx, IExpression original, IExpression candidate)
    {
        return ctx.Validation switch
        {
            ValidationMode.None => true,
            ValidationMode.Partial => MatchesKnown(ctx,original, candidate, true),
            ValidationMode.All =>  MatchesKnown(ctx, original, candidate, false),
            ValidationMode.Custom => ctx.CustomValidator?.Invoke(original, candidate) ?? true,
            _ => true
        };
    }

    private static bool MatchesKnown(ExpressionSolveContext ctx, IExpression orig, IExpression cand, bool onlyValidaHole)
    {
        if (orig.Length != cand.Length)
        {
            return false;
        }

        return orig.Length switch
        {
            5 => MatchesKnown5(ctx, (Expression5)orig, (Expression5)cand, onlyValidaHole),
            7 => MathesKnown7(ctx, (Expression7)orig, (Expression7)cand, onlyValidaHole),
            _ => false
            
        };
    }

    private static bool MatchesKnown5(ExpressionSolveContext ctx, Expression5 orig, Expression5 cand, bool onlyValidaHole)
    {
        return true;
    }

    private static bool MathesKnown7(ExpressionSolveContext ctx, Expression7 orgi, Expression7 cand, bool onlyValidaHole)
    {
        return true;
    }
}