namespace CrossMath.Core.HoleDiggers.HoleCount;
/// <summary>
/// 根据 HoleCountType 枚举规则计算期望挖空数的专用计算器
/// </summary>
public class RelativeFormulaHoleCountCalculator : IExpectedHollowCountCalculator
{
    public HoleCountType HoleType { get; }

    public RelativeFormulaHoleCountCalculator(HoleCountType holeType)
    {
        HoleType = holeType;
    }

    public int Calculate(int minHoleCount, int maxHoleCount, int formulaCount)
    {
        int raw = HoleType switch
        {
            HoleCountType.MinCount              => minHoleCount,
            HoleCountType.FormulaCountPlus1     => formulaCount + 1,
            HoleCountType.FormulaCountPlus2     => formulaCount + 2,
            HoleCountType.FormulaCountPlus3     => formulaCount + 3,
            HoleCountType.FormulaCountMinus2    => formulaCount - 2,
            HoleCountType.FormulaCountMinus1    => formulaCount - 1,
            HoleCountType.MaxCount              => maxHoleCount,

            _ => throw new NotSupportedException($"不支持的 HoleCountType: {HoleType}")
        };

        // 注意：这里不 Clamp，由 ExpectedHollowCountState 统一处理
        return raw;
    }
}