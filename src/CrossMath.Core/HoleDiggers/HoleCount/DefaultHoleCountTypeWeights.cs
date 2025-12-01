namespace CrossMath.Core.HoleDiggers.HoleCount;
/// <summary>
/// 默认权重配置管理类
/// </summary>
public static class DefaultHoleCountWeights
{
    /// <summary>
    /// 获取默认的权重配置字典
    /// </summary>
    /// <returns>默认的权重配置字典</returns>
    public static Dictionary<HoleCountType, int> GetDefaultWeights()
    {
        return new Dictionary<HoleCountType, int>
        {
            { HoleCountType.MinCount , 1},
            { HoleCountType.FormulaCountPlus1, 2 },
            { HoleCountType.FormulaCountPlus2, 2 },
            { HoleCountType.FormulaCountPlus3, 3 },
            { HoleCountType.FormulaCountMinus2, 3 },
            { HoleCountType.FormulaCountMinus1, 2 },
            { HoleCountType.MaxCount, 1 }
        };
    }

    /// <summary>
    /// 获取指定HoleCountType的默认权重值
    /// </summary>
    /// <param name="type">权重类型</param>
    /// <returns>对应的默认权重值</returns>
    public static int GetDefaultWeight(HoleCountType type)
    {
        return type switch
        {
            HoleCountType.MinCount => 1,
            HoleCountType.FormulaCountPlus1 => 2 ,
            HoleCountType.FormulaCountPlus2 => 2 ,
            HoleCountType.FormulaCountPlus3 => 3 ,
            HoleCountType.FormulaCountMinus2 => 3 ,
            HoleCountType.FormulaCountMinus1 => 2 ,
            HoleCountType.MaxCount => 1,
            _ => 0 // 默认返回最大权重
        };
    }

    /// <summary>
    /// 检查指定的权重字典是否包含所有必需的权重键
    /// </summary>
    /// <param name="weights">要检查的权重字典</param>
    /// <returns>是否包含所有必需的权重键</returns>
    public static bool ContainsAllRequiredKeys(Dictionary<HoleCountType, int> weights)
    {
        if (weights == null)
            return false;

        var requiredKeys = new[]
        {
            HoleCountType.MinCount,
            HoleCountType.FormulaCountPlus1,
            HoleCountType.FormulaCountPlus2,
            HoleCountType.FormulaCountPlus3,
            HoleCountType.FormulaCountMinus2,
            HoleCountType.FormulaCountMinus1,
            HoleCountType.MaxCount
        };

        return requiredKeys.All(key => weights.ContainsKey(key));
    }
}