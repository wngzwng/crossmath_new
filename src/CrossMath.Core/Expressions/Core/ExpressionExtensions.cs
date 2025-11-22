using CrossMath.Core.Expressions.Schema;
using CrossMath.Core.Types;
namespace CrossMath.Core.Expressions.Core;

public static class ExpressionExtensions
{
    /// <summary>
    /// 数字位还剩几个空（只统计 CellType.Number 的位置）
    /// </summary>
    public static int EmptyNumberCount(this IExpression expr)
        => expr.CountEmptyCells(CellType.Number);

    /// <summary>
    /// 运算符位还剩几个空（只统计 CellType.Operator 的位置）
    /// </summary>
    public static int EmptyOperatorCount(this IExpression expr)
        => expr.CountEmptyCells(CellType.Operator);

    /// <summary>
    /// 总空格数（不含等号格子）
    /// </summary>
    public static int EmptyCellCount(this IExpression expr)
        => expr.EmptyNumberCount() + expr.EmptyOperatorCount();

    /// <summary>
    /// 是否有运算符空格（不含等号格子）
    /// </summary>
    public static bool HasEmptySymbol(this IExpression expr)
        => expr.EmptyOperatorCount() > 0;
    
    /// <summary>
    /// 是否所有格子都已填满（不含等号也算填满）
    /// </summary>
    public static bool IsFullyFilled(this IExpression expr)
        => expr.EmptyCellCount() == 0;

    // ===== 私有辅助：统一遍历，避免重复写 LINQ =====
    private static int CountEmptyCells(this IExpression expr, CellType targetType)
    {
        int count = 0;
        var schema = ExpressionSchemaFactory.Create(expr.Length);
        for (int i = 0; i < expr.Length; i++)
        {
            if (schema.GetCellType(i) == targetType && string.IsNullOrEmpty(expr[i]))
                count++;
        }
        return count;
    }
}