using CrossMath.Core.Types;

namespace CrossMath.Core.Evaluation.LevelDifficulty.EvaluationTraces.MaxWeight;







public sealed class MaxWeightPathTrace : IEvaluationTrace
{
    public MaxWeightPath Path { get; } = new();

    private readonly int _difficultyThreshold;
    private readonly int _candidateThreshold;

    public MaxWeightPathTrace(int difficultyThreshold = 2, int candidateThreshold = 3)
    {
        _difficultyThreshold = difficultyThreshold;
        _candidateThreshold = candidateThreshold;
    }

    public void OnRunStart(int runIndex) { }

    public void OnStepEvaluated(EvaluationStep step)
    {
        var isPain = IsPainPoint(step);

        Path.Add(new MaxWeightStep(
            step.StepIndex,
            step.Coord,
            step.LocalDifficulty,
            step.Score,
            step.BoardCandidateUniqueCount,
            isPain
        ));
    }

    public void OnRunEnd(int runIndex, double finalScore) { }

    /// <summary>
    /// 卡点定义：<br/>
    /// ● 深度大于2的格子均为卡点<br/>
    /// ● 深度为1的格子均不是卡点<br/>
    /// ● 深度为2的格子<br/>
    ///     ○ 不重复备选答案数>3时为卡点<br/>
    ///     ○ 不重复备选答案数≤3时不为卡点<br/>
    /// </summary>
    private bool IsPainPoint(EvaluationStep step)
    {
        if (step.LocalDifficulty > _difficultyThreshold)
            return true;

        if (step.LocalDifficulty == _difficultyThreshold &&
            step.BoardCandidateUniqueCount > _candidateThreshold)
            return true;

        return false;
    }
}
