using CrossMath.Core.Types;

namespace CrossMath.Core.Evaluation.LevelDifficulty.EvaluationTraces.MaxWeight;

public sealed class MaxWeightPath
{
    private readonly List<MaxWeightStep> _steps = new();
    public IReadOnlyList<MaxWeightStep> Steps => _steps;

    public void Add(MaxWeightStep step)
    {
        _steps.Add(step);
    }

    public IReadOnlyList<int> PainStepIndices =>
        _steps
            .Where(s => s.IsPainPoint)
            .Select(s => s.StepIndex)
            .ToList();

    public void Clear()
    {
        _steps.Clear();
    }
}

