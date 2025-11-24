using CrossMath.Core.Types;

namespace CrossMath.Core.ExpressionSolvers.OperatorPools;


/// <summary>
/// 运算符池接口（与 INumberPool 完全对齐的设计哲学）
/// 支持：有限池、无限池、多重集（同一运算符可多次使用）、不放回验证等
/// </summary>
public interface IOperatorPool
{
    /// <summary>
    /// 去重后的所有可用运算符（用于快速遍历种类）
    /// </summary>
    IEnumerable<OpType> UniqueOperators { get; }

    /// <summary>
    /// 原始完整序列（包含重复，用于按权重顺序采样）
    /// 例如：[+, +, -, ×] 表示加号权重更高
    /// </summary>
    IEnumerable<OpType> AllOperators { get; }

    /// <summary>
    /// 是否包含某个运算符（不关心次数）
    /// </summary>
    bool Contains(OpType op);

    /// <summary>
    /// 获取某个运算符可用的次数（权重）
    /// 返回 int.MaxValue 表示“无限可用”
    /// </summary>
    int GetCount(OpType op);

    /// <summary>
    /// 是否为无限池（所有运算符都可以无限次使用）
    /// 例如：基础四则运算通常就是无限池
    /// </summary>
    bool IsInfinite { get; }
}