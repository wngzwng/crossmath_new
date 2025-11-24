using CrossMath.Core.Types;

namespace CrossMath.Core.ExpressionSolvers.OperatorPools;

public interface IOperatorPool
{
    IEnumerable<OpType> AllowOperators { get; }
}