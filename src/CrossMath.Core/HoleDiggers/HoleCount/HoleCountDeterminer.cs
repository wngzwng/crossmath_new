using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.HoleDiggers.HoleCount;


public class HoleCountDeterminer
{
    /// <summary>
    /// 使用默认权重计算挖空数量
    /// </summary>
    public int GetHoleCount(BoardData board)
    {
        var defaultWeights = CreateDefaultWeights();
        return CalculateHoleCount(board, defaultWeights);
    }

    /// <summary>
    /// 使用HoleCountType键的权重字典计算挖空数量
    /// </summary>
    public int GetHoleCount(BoardData board, Dictionary<HoleCountType, int> weights)
    {
        if (weights == null || weights.Count == 0)
            throw new ArgumentException("权重字典不能为空");

        return CalculateHoleCount(board, weights);
    }

    /// <summary>
    /// 使用算式数量作为键的权重字典计算挖空数量
    /// </summary>
    public int GetHoleCount(BoardData board, Dictionary<int, int> weights)
    {
        if (weights == null || weights.Count == 0)
            throw new ArgumentException("权重字典不能为空");

        return CalculateHoleCountWithNumericKeys(board, weights);
    }

    /// <summary>
    /// 使用指定的固定挖空数量
    /// </summary>
    public int GetHoleCount(int fixedCount)
    {
        if (fixedCount <= 0)
            throw new ArgumentException("挖空数量必须大于0");

        return fixedCount;
    }

    private int CalculateHoleCount(BoardData board, Dictionary<HoleCountType, int> weights)
    {
        int formulaCount = CalculateTotalFormulaCount(board);
        int weight = GetWeightByFormulaCount(formulaCount, weights);
        var (minCount, maxCount) = DefaultHoleCountRange.GetHoleCountRange(new Size(board.Width, board.Height));
        return CalculateQuantityFromWeight(weight, minCount, maxCount);
    }

    private int CalculateHoleCountWithNumericKeys(BoardData board, Dictionary<int, int> weights)
    {
        int formulaCount = CalculateTotalFormulaCount(board);
        if (!weights.TryGetValue(formulaCount, out int weight))
        {
            // 如果字典中没有对应的算式数量，使用默认逻辑
            // var typeWeights = ConvertNumericWeightsToTypeWeights(weights);
            // weight = GetWeightByFormulaCount(formulaCount, typeWeights);
        }
        var (minCount, maxCount) = DefaultHoleCountRange.GetHoleCountRange(new Size(board.Width, board.Height));
        return CalculateQuantityFromWeight(weight, minCount, maxCount);
    }

    private int GetWeightByFormulaCount(int formulaCount, Dictionary<HoleCountType, int> weights)
    {
        return formulaCount switch
        {
            >= 3 => weights.GetValueOrDefault(HoleCountType.FormulaCountPlus3),
            2 => weights.GetValueOrDefault(HoleCountType.FormulaCountPlus2),
            1 => weights.GetValueOrDefault(HoleCountType.FormulaCountPlus1),
            -1 => weights.GetValueOrDefault(HoleCountType.FormulaCountMinus1),
            -2 => weights.GetValueOrDefault(HoleCountType.FormulaCountMinus2),
     
        };
    }

    private Dictionary<HoleCountType, int> CreateDefaultWeights()
    {
        return DefaultHoleCountWeights.GetDefaultWeights();
    }

    private int CalculateQuantityFromWeight(int weight, int minHoleCount, int maxHoleCount)
    {
        // 使用配置的最小和最大挖空数，而不是硬编码的常量
        int quantity = minHoleCount + (weight - 1) * (maxHoleCount - minHoleCount) / 6;
        return Math.Max(minHoleCount, Math.Min(maxHoleCount, quantity));
    }

    private int CalculateTotalFormulaCount(BoardData board)
    {
        var layouts = ExpressionLayoutBuilder.ExtractLayouts(board.Layout, [5, 7]);
        return layouts.Count;
    }

}

// 为Dictionary添加GetValueOrDefault扩展方法
public static class DictionaryExtensions
{
    public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out var value) ? value : default(TValue);
    }
}