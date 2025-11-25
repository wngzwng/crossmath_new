using CrossMath.Core.Types;

namespace CrossMath.Core.Expressions.Core;

public static class Expression7Extensions
{
    public static Expression7 WithOperator(this Expression7 exp, OpType op1, OpType op2)
    {
        var clone = (Expression7)exp.Clone();
        clone.Op1 = op1;
        clone.Op2 = op2;
        return clone;
    }
}