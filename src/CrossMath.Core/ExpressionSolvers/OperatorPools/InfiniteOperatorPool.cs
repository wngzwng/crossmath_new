using CrossMath.Core.Types;

namespace CrossMath.Core.ExpressionSolvers.OperatorPools;

public sealed class InfiniteOperatorPool : IOperatorPool
{
    
    private readonly HashSet<OpType> _set;

    public InfiniteOperatorPool(IEnumerable<OpType> operators)
    {
        _set = new HashSet<OpType>(operators);
    }

    public IEnumerable<OpType> UniqueOperators => _set;
    public IEnumerable<OpType> AllOperators => _set; // 无限池，逻辑上相等
    public bool Contains(OpType op) => _set.Contains(op);
    public int GetCount(OpType op) => _set.Contains(op) ? int.MaxValue : 0;
    public bool IsInfinite => true;
}