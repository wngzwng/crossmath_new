using CrossMath.Core.Types;

namespace CrossMath.Core.ExpressionSolvers.OperatorPools;

public class OperatorPool: IOperatorPool
{
    public static OperatorPool ASMD = new OperatorPool([OpType.Add, OpType.Sub, OpType.Div, OpType.Mul]);

    public static OperatorPool ASM = new OperatorPool([OpType.Add, OpType.Sub, OpType.Mul]);
    
    public static OperatorPool AS = new OperatorPool([OpType.Add, OpType.Sub]);
    
    
    private OpType[] _ops;

    public OperatorPool(IEnumerable<OpType> opTypes)
    {
        _ops = opTypes.Distinct().ToArray();
    }
    public IEnumerable<OpType> AllowOperators => _ops;
}