using CrossMath.Core.Types;

namespace CrossMath.Core.Evaluation.LocalDifficulty;

public sealed class LocalDifficultyEvaluation
{
    public IReadOnlyDictionary<RowCol, int> MinDifficultyPerCell { get; init; } = new Dictionary<RowCol, int>();

    public IReadOnlyDictionary<RowCol, HashSet<string>> CandidateMapAtCell { get; init; }
        = new Dictionary<RowCol, HashSet<string>>();
}
