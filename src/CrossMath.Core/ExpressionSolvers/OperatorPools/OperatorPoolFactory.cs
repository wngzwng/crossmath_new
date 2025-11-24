using CrossMath.Core.Types;

namespace CrossMath.Core.ExpressionSolvers.OperatorPools;


// 3. 工厂（清晰、统一、对称、优雅到极致）
public static class OperatorPoolFactory
{
    // 常用无限池（全局单例）
    /// <summary>
    /// 加减乘除
    /// </summary>
    public static IOperatorPool ASMD   => Infinite(OpType.Add, OpType.Sub, OpType.Mul, OpType.Div);
    /// <summary>
    /// 加减乘
    /// </summary>
    public static IOperatorPool ASM    => Infinite(OpType.Add, OpType.Sub, OpType.Mul);
    /// <summary>
    /// 加减
    /// </summary>
    public static IOperatorPool AS     => Infinite(OpType.Add, OpType.Sub);
    
    public static IOperatorPool Infinite(params OpType[] ops) => new InfiniteOperatorPool(ops);
    public static IOperatorPool Infinite(IEnumerable<OpType> ops) => new InfiniteOperatorPool(ops);

    public static IOperatorPool Discrete(params OpType[] ops) => new DiscreteOperatorPool(ops);
    public static IOperatorPool Discrete(IEnumerable<OpType> ops) => new DiscreteOperatorPool(ops);
}