using CrossMath.Core.Types;

namespace CrossMath.Core.Evaluation.LevelDifficulty.ScoreStrategies;

public interface IScoreStrategy
{
    IReadOnlyDictionary<RowCol, double> Score(
        LevelDifficultyContext ctx,
        RowCol lastCoord,
        IReadOnlyDictionary<RowCol, int> localDifficulties,
        IReadOnlyDictionary<RowCol, HashSet<string>> candidateMapAtCell
        );
}