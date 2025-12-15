using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Types;
using CrossMath.Core.Utils;

namespace CrossMath.Core.Generators.PlacementOrderingPolicies;

/// <summary>
/// “扩张平衡”排序策略（Fail-First / Worst-First） 
/// </summary>
public class PlacementOrderingPolicy : IPlacementOrderingPolicy
{
    public IEnumerable<Placement> Order(
        IEnumerable<Placement> placements,
        ICanvas currentCanvas,
        Random? random)
    {
        // 1. 扩张越大越先评估（快速建立低 alpha，后面大量分支直接剪枝）
        // 2. 扩张相同时，sigma 越小（越平整）越靠前
        return placements
            .OrderByDescending(p => CalculateExpansionScore(p, currentCanvas))
            .ThenBy(p => CalculateSigmaWithPlacement(p, currentCanvas));
    }

    /// <summary>
    /// 扩张程度评分 —— 数值越大表示放置后画布越“膨胀”，越应该先被评估
    /// </summary>
    private static int CalculateExpansionScore(Placement placement, ICanvas canvas)
    {
        Size newSize = GetCanvasSizeWithPlacement(placement, canvas);
        return newSize.Height + newSize.Width;

        // 如需更激进的策略，可直接改成下面任意一种（只改这里即可）：
        // return newSize.Area - canvas.CanvasSize.Area;                              // 扩张面积
        // return Math.Max(newSize.Height - canvas.CanvasSize.Height,
        //                 newSize.Width  - canvas.CanvasSize.Width) * 100;           // 最大单向扩张
    }

    /// <summary>
    /// 计算放置该 Placement 后画布会变成的尺寸（不实际修改 canvas）
    /// </summary>
    private static Size GetCanvasSizeWithPlacement(Placement placement, ICanvas canvas)
    {
        // 当前已占格子
        var occupied = canvas.CanvasSize
                             .AllPositions()
                             .Where(canvas.HasValue)
                             .ToList();                     // ToList 避免多次枚举

        // 加上本次要放置的格子
        occupied.AddRange(placement.EnumerateCellsAsRowCol());

        var (min, max) = occupied.GetMinMaxPosition();
        return Size.GetBoundingBoxSize(min, max);
    }

    /// <summary>
    /// 计算放置该 Placement 后四个象限的填充数 → sigma 值（不修改原 canvas）
    /// </summary>
    private static double CalculateSigmaWithPlacement(Placement placement, ICanvas canvas)
    {
        var quadrantCounts = new int[4];

        int index = 0;
        foreach (var (start, end) in canvas.CanvasSize.GetQuadrants())
        {
            int filled = Size.TraverseSection(start, end)
                             .Count(rc => canvas.HasValue(rc) || placement.Contains(rc));

            quadrantCounts[index++] = filled;
        }

        return MathMisc.CalcSigma(quadrantCounts);
    }
}