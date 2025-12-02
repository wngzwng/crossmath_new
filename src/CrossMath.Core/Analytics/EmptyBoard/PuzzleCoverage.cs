using CrossMath.Core.Types;

namespace CrossMath.Core.Analytics.EmptyBoard;

public static class PuzzleCoverage
{
    public static double CalcCoverage(
        int formulaNum, 
        int height, 
        int width,
        FormulaCapacityRegistry registry)
    {
        if (!registry.TryGetMaxFormula(height, width, out int maxFormula))
            return 0;

        if (maxFormula <= 0) 
            return 0;

        double ratio = (double)formulaNum / maxFormula;
        return Math.Round(ratio, 2);
    }
    
    public static double CalcCoverage(
        int formulaNum, 
        Size size, 
        FormulaCapacityRegistry registry)
    {
        return CalcCoverage(formulaNum, size.Height, size.Width, registry);
    }
}
