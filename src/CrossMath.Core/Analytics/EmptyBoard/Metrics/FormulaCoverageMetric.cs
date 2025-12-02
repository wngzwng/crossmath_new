using CrossMath.Core.Analytics.EmptyBoard.Pipeline;
using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Models;

namespace CrossMath.Core.Analytics.EmptyBoard.Metrics;

public class FormulaCoverageMetric : IBoardMetric
{
    public string Name => MetricNames.FormulaCoverage;

    // --- 独立使用的核心方法（随时调用，无需 pipeline） ---
    public static double Calculate(BoardLayout layout, FormulaCapacityRegistry registry)
    {
        int formulaNum = ExpressionLayoutBuilder.ExtractLayouts(layout, [5, 7]).Count;
        int row = layout.Height;
        int col = layout.Width;

        if (!registry.TryGetMaxFormula(row, col, out int max) || max <= 0)
            return 0.0;

        return Math.Round((double)formulaNum / max, 2);
    }

    // --- pipeline 使用 ---
    public void Compute(BoardLayout layout, BoardMetricsContext ctx, BoardMetricsResult result)
    {
        double value = Calculate(layout, ctx.CapacityRegistry!);
        result.Set(Name, value);
    }
}
