using CrossMath.Core.Types;

namespace CrossMath.Core.Evaluation.LevelDifficulty.SelectionPolicies;

public interface ISelectionPolicy
{
    RowCol Select(IReadOnlyDictionary<RowCol, double> weights);
}