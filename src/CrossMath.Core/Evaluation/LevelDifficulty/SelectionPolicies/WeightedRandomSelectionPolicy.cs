using CrossMath.Core.Types;

namespace CrossMath.Core.Evaluation.LevelDifficulty.SelectionPolicies;

/// <summary>
/// 按权重随机选择坐标
/// </summary>
public sealed class WeightedRandomSelectionPolicy : ISelectionPolicy
{
    private readonly Random _rng;

    public WeightedRandomSelectionPolicy(Random? random = null)
    {
        _rng = random ?? new Random();
    }
    public RowCol Select(IReadOnlyDictionary<RowCol, double> weights)
    {
        if (weights.Count == 0)
            throw new InvalidOperationException(
                "SelectionPolicy.Select called with empty weight set");

        if (weights.Count == 1)
            return weights.First().Key;

        double total = weights.Values.Sum();
        if (total <= 0)
            throw new InvalidOperationException(
                "All weights are zero or negative");
        
        double r = _rng.NextDouble() * total;

        foreach (var (coord, weight) in weights)
        {
            r -= weight;
            if (r <= 0)
                return coord;
        }

        return weights.Last().Key;
    }
}
