using CrossMath.Core.Types;

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
    /// 排序方式：升序 / 降序 / 乱序
    /// </summary>
    NumberOrder Order { get; set; }

    /// <summary>
    /// 是否包含指定数字
    /// </summary>
    bool Contains(int number);

    /// <summary>
    /// 获取某个数字可使用的次数（权重）
    /// </summary>
    int GetCount(int number);

    /// <summary>
    /// 是否为无限池
    /// </summary>
    bool IsInfinite { get; }
}