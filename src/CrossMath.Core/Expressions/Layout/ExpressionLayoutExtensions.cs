using CrossMath.Core.Models;
using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Expressions.Core;
using CrossMath.Core.Types;

namespace CrossMath.Core.Expressions.Layout;

public static class ExpressionLayoutExtensions
{
    /// <summary>
    /// 数字位还剩几个空（只统计 CellType.Number 的位置）
    /// </summary>
    public static int EmptyNumberCount(this ExpressionLayout exprLayout, BoardData board)
        => exprLayout.CountEmptyCells(board, CellType.Number);

    /// <summary>
    /// 运算符位还剩几个空（只统计 CellType.Operator 的位置）
    /// </summary>
    public static int EmptyOperatorCount(this ExpressionLayout exprLayout, BoardData board)
        => exprLayout.CountEmptyCells(board, CellType.Operator);

    /// <summary>
    /// 总空格数（不含等号格子）
    /// </summary>
    public static int EmptyCellCount(this ExpressionLayout exprLayout, BoardData board)
        => exprLayout.EmptyNumberCount(board) + exprLayout.EmptyOperatorCount(board);

    /// <summary>
    /// 是否有运算符空格（不含等号格子）
    /// </summary>
    public static bool HasEmptySymbol(this ExpressionLayout exprLayout, BoardData board)
        => exprLayout.EmptyOperatorCount(board) > 0;

    /// <summary>
    /// 是否所有格子都已填满（不含等号也算填满）
    /// </summary>
    public static bool IsFullyFilled(this ExpressionLayout exprLayout, BoardData board)
        => exprLayout.EmptyCellCount(board) == 0;

    // ===== 私有辅助：统一遍历，避免重复写 LINQ =====
    private static int CountEmptyCells(this ExpressionLayout exprLayout, BoardData board, CellType targetType)
    {
        int count = 0;
        for (int i = 0; i < exprLayout.Length; i++)
        {
            if (exprLayout.Schema.GetCellType(i) == targetType && string.IsNullOrEmpty(board[exprLayout[i]]))
                count++;
        }

        return count;
    }


    /// <summary>
    /// 提取表达式（即使有空位也返回部分填充的表达式）
    /// </summary>
    public static IExpression ToExpression(this ExpressionLayout exprLayout, BoardData board)
    {
        var tokens = exprLayout.Cells
            .Select(pos => board[pos]) // 空位用空字符串占位
            .ToArray();

        return ExpressionFactory.FromTokens(tokens);
    }
}