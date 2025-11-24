namespace CrossMath.Core.ExpressionSolvers.NumberPools;

/// <summary>
/// 数字池接口：统一抽象有限池与无限池的行为
/// 支持多重集（重复数字）、不放回抽样验证、权重查询等
/// </summary>
public interface INumberPool
{
    /// <summary>
    /// 去重后的所有数字种类（用于快速遍历可用数字）
    /// </summary>
    IEnumerable<int> UniqueNumbers { get; }

    /// <summary>
    /// 原始完整序列（包含重复，用于采样算法按权重顺序遍历）
    /// </summary>
    IEnumerable<int> AllNumbers { get; }

    /// <summary>
    /// 是否包含指定数字（不关心次数）
    /// </summary>
    bool Contains(int number);

    /// <summary>
    /// 获取某个数字在池子中可用的次数（权重）
    /// 返回 int.MaxValue 表示“无限可用”
    /// </summary>
    int GetCount(int number);

    /// <summary>
    /// 是否为无限池（每个数字可无限次使用）
    /// 强烈建议所有实现类显式实现此属性，避免算法误判
    /// </summary>
    bool IsInfinite { get; }
    
}