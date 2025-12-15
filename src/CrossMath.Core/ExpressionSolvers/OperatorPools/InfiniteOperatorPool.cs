using CrossMath.Core.Types;

namespace CrossMath.Core.ExpressionSolvers.OperatorPools;

public sealed class InfiniteOperatorPool : IOperatorPool
{
    
    private readonly HashSet<OpType> _set;
    private readonly List<OpType> _ops;

    public InfiniteOperatorPool(IEnumerable<OpType> operators)
    {
        _ops = operators.Distinct().ToList();
        _set = new HashSet<OpType>(_ops);
    }

    public IEnumerable<OpType> UniqueOperators => _ops;
    public IEnumerable<OpType> AllOperators => _set; // 无限池，逻辑上相等
    public bool Contains(OpType op) => _set.Contains(op);
    public int GetCount(OpType op) => _set.Contains(op) ? int.MaxValue : 0;
    public bool IsInfinite => true;

    public override string ToString()
    {
       return  $"Infinite operators: {string.Join(", ", _set)}";
    }
}