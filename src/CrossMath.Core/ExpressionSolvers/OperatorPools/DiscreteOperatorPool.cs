using CrossMath.Core.Types;

namespace CrossMath.Core.ExpressionSolvers.OperatorPools;

public class DiscreteOperatorPool: IOperatorPool
{
    // 2. 有限离散运算符池（支持权重，比如 + 出现3次，- 出现1次）
 
    private readonly IReadOnlyList<OpType> _source;

    public DiscreteOperatorPool(IEnumerable<OpType> source)
    {
        _source = source?.ToArray() ?? throw new ArgumentNullException();
    }

    public IEnumerable<OpType> UniqueOperators => _source.Distinct();
    public IEnumerable<OpType> AllOperators => _source;
    public bool Contains(OpType op) => _source.Contains(op);
    public int GetCount(OpType op) => _source.Count(x => x == op);
    public bool IsInfinite => false;
    
    public override string ToString()
    {
        return  $"DiscreteOperatorPool operators: {string.Join(", ", _source)}";
    }
    
}