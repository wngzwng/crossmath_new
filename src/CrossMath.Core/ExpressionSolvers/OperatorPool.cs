using CrossMath.Core.Types;

namespace CrossMath.Core.ExpressionSolvers;

public class OperatorPool
{
    public IEnumerable<OpType> AllowOperators => [OpType.Add, OpType.Sub, OpType.Div, OpType.Mul];
}