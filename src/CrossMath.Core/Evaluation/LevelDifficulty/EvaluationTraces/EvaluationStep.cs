using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Evaluation.LevelDifficulty.EvaluationTraces;

public sealed record EvaluationStep
(
    int StepIndex,
    RowCol Coord,
    int LocalDifficulty,
    double Score,
    int BoardCandidateUniqueCount,
    IReadOnlyDictionary<RowCol, int> LocalDifficultySnapshot
);
