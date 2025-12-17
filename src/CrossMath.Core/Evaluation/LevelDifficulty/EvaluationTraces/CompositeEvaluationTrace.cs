namespace CrossMath.Core.Evaluation.LevelDifficulty.EvaluationTraces;

public sealed class CompositeEvaluationTrace : IEvaluationTrace
{
    private readonly IReadOnlyList<IEvaluationTrace> _traces;

    public CompositeEvaluationTrace(params IEvaluationTrace[] traces)
    {
        _traces = traces;
    }

    public void OnRunStart(int runIndex)
    {
        foreach (var t in _traces)
            t.OnRunStart(runIndex);
    }

    public void OnStepEvaluated(EvaluationStep step)
    {
        foreach (var t in _traces)
            t.OnStepEvaluated(step);
    }

    public void OnRunEnd(int runIndex, double finalScore)
    {
        foreach (var t in _traces)
            t.OnRunEnd(runIndex, finalScore);
    }
}