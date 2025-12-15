using CrossMath.Core.Utils;

namespace CrossMath.Core.HoleDiggers.HoleCount;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 根据权重随机选择 HoleCountType 的工具类
/// 支持默认权重和自定义权重，权重越高被选中的概率越大
/// </summary>
public static class RandomHoleCountTypeSelector
{
    private static readonly Random _random = new();
    
    /// <summary>
    /// 默认权重配置（可外部引用，但建议通过方法访问以保持一致性）
    /// </summary>
    private static readonly Dictionary<HoleCountType, int> DefaultWeights = new Dictionary<HoleCountType, int>
    {
        { HoleCountType.MinCount,             1 },
        { HoleCountType.FormulaCountPlus1,    2 },
        { HoleCountType.FormulaCountPlus2,    2 },
        { HoleCountType.FormulaCountPlus3,    3 },
        { HoleCountType.FormulaCountMinus2,   3 },
        { HoleCountType.FormulaCountMinus1,   2 },
        { HoleCountType.MaxCount,             1 }
    };
    
    private static readonly WeightedRandom<HoleCountType> defaultWeightedRandom = WeightedRandom.From(DefaultWeights);

    /// <summary>
    /// 获取默认的权重配置字典（只读副本）
    /// </summary>
    public static Dictionary<HoleCountType, int> GetDefaultWeights()
    {
        return new Dictionary<HoleCountType, int>(DefaultWeights);
    }

    /// <summary>
    /// 获取指定 HoleCountType 的默认权重值
    /// </summary>
    public static int GetDefaultWeight(HoleCountType type)
    {
        return DefaultWeights.TryGetValue(type, out int weight) ? weight : 0;
    }

    /// <summary>
    /// 检查权重字典是否包含所有必需的 HoleCountType 键
    /// </summary>
    public static bool ContainsAllRequiredKeys(Dictionary<HoleCountType, int>? weights)
    {
        if (weights == null || weights.Count == 0)
            return false;

        return DefaultWeights.Keys.All(key => weights.ContainsKey(key));
    }

    // ======================== 随机选择核心方法 ========================

    /// <summary>
    /// 使用默认权重随机选择一个 HoleCountType
    /// </summary>
    /// <returns>按权重随机选择的难度类型</returns>
    public static HoleCountType GetRandomByDefaultWeight()
    {
        return defaultWeightedRandom.Next();
    }

    /// <summary>
    /// 使用自定义权重随机选择一个 HoleCountType
    /// </summary>
    /// <param name="customWeights">自定义权重字典，必须包含所有 HoleCountType</param>
    /// <exception cref="ArgumentException">权重字典不完整或权重总和 ≤ 0 时抛出</exception>
    public static HoleCountType GetRandomByWeight(IDictionary<HoleCountType, int> customWeights)
    {
        if (customWeights == null)
            throw new ArgumentNullException(nameof(customWeights));

        if (!ContainsAllRequiredKeys(customWeights as Dictionary<HoleCountType, int>))
            throw new ArgumentException("自定义权重字典必须包含所有 HoleCountType 键", nameof(customWeights));

        int totalWeight = customWeights.Values.Sum(w => w < 0 ? 0 : w); // 防止负权重

        if (totalWeight <= 0)
            throw new InvalidOperationException("权重总和必须大于 0");

        return WeightedRandom.From(customWeights).Next();
    }

    /// <summary>
    /// 使用默认权重批量随机生成多个 HoleCountType（适合批量生成题目）
    /// </summary>
    /// <param name="count">生成数量</param>
    /// <returns>随机结果列表</returns>
    public static List<HoleCountType> GetMultipleByDefaultWeight(int count)
    {
        if (count <= 0) return new List<HoleCountType>();

        var result = new List<HoleCountType>(count);
        for (int i = 0; i < count; i++)
        {
            result.Add(GetRandomByDefaultWeight());
        }
        return result;
    }
}