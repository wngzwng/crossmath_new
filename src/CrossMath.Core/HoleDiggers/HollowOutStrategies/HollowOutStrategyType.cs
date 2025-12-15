namespace CrossMath.Core.HoleDiggers.HollowOutStrategies;

public enum HollowOutStrategyType
{
    /// <summary>
    /// 交点优先策略：优先选择行和列都有其他数字的交点位置<br/>
    /// 每个交点格权重7，每个非交点格权重3
    /// </summary>
    IntersectionPriority,

    /// <summary>
    /// 非焦点优先策略：优先选择非交点位置<br/>
    /// 每个交点格权重3，每个非交点格权重7
    /// </summary>
    NonFocusPriority,

    /// <summary>
    /// 随机策略：所有候选位置等概率选择
    /// </summary>
    Random,

    /// <summary>
    /// 重复数字多优先策略：优先挖空出现次数较多的数字<br/>
    /// 每个数字的权重为它的出现次数
    /// </summary>
    FrequentNumberPriority,

    /// <summary>
    /// 数字友好度优先策略：每个数字的权重为它的数字友好度
    /// </summary>
    NumberFriendlinessPriority,

    /// <summary>
    /// 优先大数字策略：优先挖空数值较大的数字<br/>
    /// 每个数字的权重为它的数值
    /// </summary>
    LargeNumberPriority,
    
    /// <summary>
    /// 优先小数字策略：优先挖空数值较小的数字<br/>
    /// 每个数字的权重为（1/它的数值）
    /// </summary>
    SmallNumberPriority
}