namespace CrossMath.Core.Evaluation.LevelDifficulty.EvaluationTraces;


public interface IEvaluationTrace
{
    void OnRunStart(int runIndex);
    void OnStepEvaluated(EvaluationStep step);
    void OnRunEnd(int runIndex, double finalScore);
}
