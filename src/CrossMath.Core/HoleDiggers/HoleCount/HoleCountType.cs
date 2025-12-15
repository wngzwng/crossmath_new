namespace CrossMath.Core.HoleDiggers.HoleCount;

public enum HoleCountType
{
    MinCount,               //最小算式数
    
    FormulaCountPlus1,  // 算式数+1
    FormulaCountPlus2,   // 算式数+2
    FormulaCountPlus3,    // 算式数+3

    FormulaCountMinus2, // 算式数-2
    FormulaCountMinus1, // 算式数-1

    MaxCount,           // 最大算式数
}