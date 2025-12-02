using CrossMath.Core.Analytics.EmptyBoard.Pipeline;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Analytics.EmptyBoard.Metrics;

public class CrossCountMetric: IBoardMetric
{
    public string Name => MetricNames.CrossCount;

    // --- 独立使用的核心方法（随时调用，无需 pipeline） ---
    public static int Calculate(BoardLayout layout)
    {
        return layout.ValidPositions().Count(pos => IsCross(layout, pos));
    }

    // --- pipeline 使用 ---
    public void Compute(BoardLayout layout, BoardMetricsContext ctx, BoardMetricsResult result)
    {
        int value = Calculate(layout);
        result.Set(Name, value);
    }

    private static bool IsCross(BoardLayout layout, RowCol pos)
    {
        var size = layout.BoardSize;
        if (!pos.InBounds(size)) return false;

        var hasUp = layout.IsValid(pos + RowCol.Up);
        var hasDown = layout.IsValid(pos + RowCol.Down);
        
        var hasLeft = layout.IsValid(pos + RowCol.Left);
        var hasRight = layout.IsValid(pos + RowCol.Right);
        
        return (hasUp || hasDown) && (hasLeft || hasRight);
    }
}