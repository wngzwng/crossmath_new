using CrossMath.Core.Types;

namespace CrossMath.Core.Evaluation.LevelDifficulty.SelectionPolicies;

/// <summary>
/// 选择权重最大的坐标（确定性）
/// </summary>
public sealed class MaxWeightSelectionPolicy : ISelectionPolicy
{
    public RowCol Select(IReadOnlyDictionary<RowCol, double> weights)
    {
        if (weights.Count == 0)
            throw new InvalidOperationException(
                "SelectionPolicy.Select called with empty weight set");

        return weights
            .OrderByDescending(kv => kv.Value)
            .First()
            .Key;
    }
}
