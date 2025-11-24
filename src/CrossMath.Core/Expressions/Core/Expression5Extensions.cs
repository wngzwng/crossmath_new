using CrossMath.Core.Types;

namespace CrossMath.Core.Expressions.Core;

public static class Expression5Extensions
{
    public static Expression5 WithOperator(this Expression5 exp, OpType op)
    {
        var clone = (Expression5)exp.Clone();
        clone.Op = op;
        return clone;
    }
}