using CrossMath.Core.Types;

namespace CrossMath.Core.Models;

public static class BoardDataPuzzleExtensions
{
    /// <summary>
    /// 从 PossibleAnswers 中取值填入盘面（初盘场景）
    /// </summary>
    public static bool ApplyAnswer(this BoardData board, RowCol pos, out string value)
    {
        value = string.Empty;

        if (!board.Layout.IsValid(pos))
            return false;

        var sortedHoles = board.GetSortedHoles();
        int index = sortedHoles.IndexOf(pos);

        if (index < 0 || index >= board.PossibleAnswers.Count)
            return false;

        value = board.PossibleAnswers[index];

        // 填入盘面
        board.SetValueOnly(pos, value);

        // 从答案池移除
        board.PossibleAnswers.RemoveAt(index);

        // 同步移除 hole
        board.RemoveHole(pos);

        return true;
    }

    /// <summary>
    /// 将盘面值重新挖空，放回 PossibleAnswers（挖空器的行为）
    /// </summary>
    public static bool ExtractAnswer(this BoardData board, RowCol pos, out string value)
    {
        value = string.Empty;

        if (!board.Layout.IsValid(pos))
            return false;

        if (!board.IsFilled(pos))
            return false;

        // 移除盘面值
        board.FilledValues.Remove(pos);

        // 推导类型（基于 value 字符串）
        CellType type = SymbolManager.InferCellType(value);
        board.AddHole(pos, type);

        // 插入回 PossibleAnswers
        var sortedHoles = board.GetSortedHoles();
        int index = sortedHoles.IndexOf(pos);

        board.PossibleAnswers.Insert(index, value);
        return true;
    }
}