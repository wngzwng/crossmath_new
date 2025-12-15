namespace CrossMath.Core.HoleDiggers.HoleCount;

/// <summary>
/// 期望挖空数量计算器
/// 用于根据第一阶段的最小挖空数（约束下最大可挖空数）、难度上限、算式数量等信息，
/// 计算出最终的期望挖空数量
/// </summary>
public interface IExpectedHollowCountCalculator
{
    /// <summary>
    /// 计算期望挖空数量
    /// </summary>
    /// <param name="minHoleCount">第一阶段得到的约束下最大可挖空数（即“最小挖空数”）</param>
    /// <param name="maxHoleCount">该难度配置允许的最大挖空数上限</param>
    /// <param name="formulaCount">当前题目中算式的总数</param>
    /// <returns>最终期望的挖空数量，应满足 minHoleCount ≤ result ≤ maxHoleCount</returns>
    int Calculate(int minHoleCount, int maxHoleCount, int formulaCount);
}