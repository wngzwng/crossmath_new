using CrossMath.Core.Analytics.EmptyBoard.Pipeline;
using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Models;

namespace CrossMath.Core.Analytics.EmptyBoard.Metrics;

public class Formula7Metric : IBoardMetric
{
    public string Name => MetricNames.Formula7Count;

    // --- 独立使用的核心方法（随时调用，无需 pipeline） ---
    public static int Calculate(BoardLayout layout)
    {
        var expLayouts = ExpressionLayoutBuilder.ExtractLayouts(layout, [5, 7]);
        return expLayouts.Count(expLay => expLay.Length == 7);
    }

    // --- pipeline 使用 ---
    public void Compute(BoardLayout layout, BoardMetricsContext ctx, BoardMetricsResult result)
    {
        int value = Calculate(layout);
        result.Set(Name, value);
    }
}