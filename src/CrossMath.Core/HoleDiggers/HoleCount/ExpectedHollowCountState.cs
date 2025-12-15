using CrossMath.Core.HoleDiggers.HoleCount;
using CrossMath.Core.Types;

namespace CrossMath.Core.HoleDiggers.HoleCount;

public class ExpectedHollowCountState
{
    public HoleCountType HoleType { get; set; }

    /// <summary>
    /// 该难度允许的最大挖空数（硬上限，防止过度挖空导致无解或太难）
    /// </summary>
    public int MaxHoleCount { get; private set; }

    /// <summary>
    /// 第一阶段计算出的“每个算式至少一个空”约束下的最大挖空数
    /// </summary>
    public int MinHoleCount { get; private set; }

    /// <summary>
    /// 期望挖空数量计算策略（自动根据 HoleType 创建）
    /// </summary>
    public IExpectedHollowCountCalculator Calculator { get; private set; }


    public static ExpectedHollowCountState Create(HoleCountType countType)
    {
        var state = new ExpectedHollowCountState();
        state.SetHoleType(countType);
        return state;
    }
    
    /// <summary>
    /// 设置 HoleType 时自动创建对应的计算器（最方便的使用方式）
    /// </summary>
    public void SetHoleType(HoleCountType holeType)
    {
        HoleType = holeType;
        Calculator = new RelativeFormulaHoleCountCalculator(holeType);
    }

    /// <summary>
    /// 计算最终期望挖空数
    /// </summary>
    /// <param name="actualMinHoleCount">第一阶段实际计算出的约束下最大挖空数</param>
    /// <param name="formulaCount">当前题目的算式总数</param>
    /// <returns>最终安全夹紧后的期望挖空数量</returns>
    public int CalculateExpected(int actualMinHoleCount, int formulaCount)
    {
        MinHoleCount = actualMinHoleCount;

        if (Calculator == null)
            throw new InvalidOperationException("Calculator 未设置，请先调用 SetHoleType 或手动赋值 Calculator");

        MaxHoleCount = DefaultMaxHoleCountMap.GetDefaultMaxHoleCount(formulaCount);
        
        int rawExpected = Calculator.Calculate(actualMinHoleCount, MaxHoleCount, formulaCount);

        // 关键安全保护：
        // 期望挖空数不能少于 MinHoleCount（否则会破坏“每个算式至少一个空”）
        // 也不能超过 MaxHoleCount（难度上限）
        return Math.Clamp(rawExpected, actualMinHoleCount, MaxHoleCount);
    }
    
    
}