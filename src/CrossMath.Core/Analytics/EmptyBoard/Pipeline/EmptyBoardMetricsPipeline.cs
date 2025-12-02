using CrossMath.Core.Models;

namespace CrossMath.Core.Analytics.EmptyBoard.Pipeline;

public class BoardMetricsPipeline
{
    private readonly List<IBoardMetric> _metrics = new();

    public BoardMetricsPipeline Add(IBoardMetric metric)
    {
        _metrics.Add(metric);
        return this;
    }

    public BoardMetricsResult Evaluate(BoardLayout layout, BoardMetricsContext ctx)
    {
        var result = new BoardMetricsResult();

        foreach (var m in _metrics)
            m.Compute(layout, ctx, result);

        return result;
    }
}
