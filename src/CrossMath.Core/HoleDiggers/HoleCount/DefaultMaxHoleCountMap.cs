namespace CrossMath.Core.HoleDiggers.HoleCount;

/// <summary>
/// 根据算式数量（formulaCount）提供默认的最大允许挖空数（MaxHoleCount）映射表
/// 用于在未手动指定上限时，给出合理的安全挖空上限
/// 值基于实际 CrossMath 类题目测试经验，可根据游戏平衡继续调整
/// </summary>
public static class DefaultMaxHoleCountMap
{
    // key: 算式数量, value: 推荐的最大挖空数
    private static readonly Dictionary<int, int> _baseMaxByFormula = new()
    {
        { 4,   8 },
        { 5,  10 },
        { 6,  12 },
        { 7,  14 },
        { 8,  16 },
        { 9,  18 },
        { 10, 20 },
        { 11, 21 },
        { 12, 21 },
        { 13, 23 },
        { 14, 24 },
        { 15, 24 },
        { 16, 24 },
        { 17, 21 },
        { 18, 24 },
        { 19, 24 },
        { 20, 24 },
        { 21, 24 }
    };

    // 对于超出表范围的算式数量的保守 fallback 值
    private const int DefaultFallbackMax = 24;
    
    /// <summary>
    /// 根据算式数量获取默认的最大允许挖空数
    /// </summary>
    /// <param name="formulaCount">当前题目的算式总数</param>
    /// <returns>推荐的最大挖空数</returns>
    public static int GetDefaultMaxHoleCount(int formulaCount)
    {
        if (formulaCount <= 0)
            return 0;

        // 优先精确匹配
        if (_baseMaxByFormula.TryGetValue(formulaCount, out int maxCount))
        {
            return maxCount;
        }
        // 算式数 > 21：通常题目已很复杂，不再大幅增加挖空上限
        return DefaultFallbackMax;
    }
    
}