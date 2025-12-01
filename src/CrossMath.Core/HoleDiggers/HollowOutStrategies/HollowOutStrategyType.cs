namespace CrossMath.Core.HoleDiggers.HollowOutStrategies;

public enum HollowOutStrategyType
{
    /// <summary>
    /// 交点优先策略：优先选择行和列都有其他数字的交点位置
    /// </summary>
    IntersectionPriority,

    /// <summary>
    /// 非焦点优先策略：优先选择行或列中只有一个数字的非交点位置
    /// </summary>
    NonFocusPriority,

    /// <summary>
    /// 随机策略：所有候选位置等概率选择
    /// </summary>
    Random,

    /// <summary>
    /// 重复数字多优先策略：优先挖空出现次数较多的数字
    /// </summary>
    FrequentNumberPriority,

    /// <summary>
    /// 数字友好度优先策略：优先挖空在同一行和列中已经有较多相同数字的位置
    /// </summary>
    NumberFriendlinessPriority,

    /// <summary>
    /// 优先大数字策略：优先挖空数值较大的数字
    /// </summary>
    LargeNumberPriority
}