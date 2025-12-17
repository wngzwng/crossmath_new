using CrossMath.Core.Evaluation.LevelDifficulty.EvaluationTraces.MaxWeight;

namespace CrossMath.Core.Evaluation.LevelDifficulty.Analysis;

public static class MaxWeightPathAnalyzer
{
    public static MaxWeightPathAnalysis Analyze(MaxWeightPath path)
    {
        var steps = path.Steps;
        var stuckSteps = steps.Where(s => s.IsPainPoint).ToList();

        int? first = stuckSteps.FirstOrDefault()?.StepIndex;

        return new MaxWeightPathAnalysis
        {
            StuckNum = stuckSteps.Count,
            StuckPoints = stuckSteps.Select(s => s.StepIndex).ToList(),
            FirstStuckPoint = first,
            FirstStuckPointPercent =
                first.HasValue && steps.Count > 0
                    ? (double)first.Value / steps.Count
                    : null,

            PathCoordinates = steps
                .Select(s => (s.Coord.Row, s.Coord.Col, s.LocalDifficulty))
                .ToList()
        };
    }
}