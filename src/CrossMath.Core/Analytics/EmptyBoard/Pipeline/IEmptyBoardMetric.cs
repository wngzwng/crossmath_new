using CrossMath.Core.Models;

namespace CrossMath.Core.Analytics.EmptyBoard.Pipeline;

public interface IBoardMetric
{
    string Name { get; }
    void Compute(BoardLayout layout, BoardMetricsContext ctx, BoardMetricsResult result);
}
