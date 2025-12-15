using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.HoleDiggers.HollowOutStrategies;

/// <summary>
/// 挖空策略：决定下一个要挖空的坐标
/// </summary>
public interface IHollowOutStrategy
{
    /// <summary>
    /// 获取下一个建议挖空的坐标
    /// </summary>
    /// <param name="context">挖空上下文</param>
    /// <param name="candidatePositions">
    /// 可选的候选坐标集合。
    /// 为 null 或空时表示在整个棋盘范围内挑选（默认行为）。
    /// 传入具体坐标时，只在这几个坐标中挑选（常用于“只挖某些区域”等场景）。
    /// 例如第一阶段，每个算式一个空格时，范围就限制在这个算式所在的坐标
    /// </param>
    /// <returns>建议挖空的坐标，没有合适的返回 null</returns>
    RowCol? GetNextHoleCoordinate(
        HollowOutContext context,
        IEnumerable<RowCol>? candidatePositions = null);

    bool AllowHollowOutOperator { get; }
}