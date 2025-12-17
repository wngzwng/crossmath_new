using CrossMath.Core.Types;

namespace CrossMath.Core.Evaluation.LevelDifficulty.EvaluationTraces.MaxWeight;

public sealed record MaxWeightStep
(
    int StepIndex,                 // path index (0-based)
    RowCol Coord,
    int LocalDifficulty,
    double Score,
    int BoardCandidateUniqueCount,
    bool IsPainPoint
);