using CrossMath.Core.Types;

namespace CrossMath.Core.Evaluation.LevelDifficulty.WeightCalculators;

public interface IWeightCalculator
{
    IReadOnlyDictionary<RowCol, double> Calculate(
        LevelDifficultyContext context,
        IReadOnlyDictionary<RowCol, double> scores);
}