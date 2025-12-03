using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Types;

namespace CrossMath.Core.Generators.PlacementOrderingPolicies;

/// <summary>
/// 放置点排序策略
/// 只负责对 Generate() 返回的 Placement 列表进行优先级排序
/// 不过滤、不修改、纯函数、无副作用、可热插拔
/// </summary>
public interface IPlacementOrderingStrategy
{
    IEnumerable<Placement> Order(
        IEnumerable<Placement> placements,
        ICanvas currentCanvas,
        Random random);
}