using System.Text;
using CrossMath.Core.Types;

namespace CrossMath.Core.Models;

public static class BoardDataExtensions
{
    // -----------------------------
    // Board Kind 推导
    // -----------------------------
    public static BoardKind  GetKind(this BoardData board)
    {
        int total   = board.Layout.ValidCellCount;
        int filled  = board.FilledValues.Count;
        int holes   = board.Holes.Count;
        int answers = board.PossibleAnswers.Count;

        if (filled == 0 && holes == total && answers == 0)
            return BoardKind.LayoutOnly;

        if (filled + holes == total && answers == 0 && filled < total)
            return BoardKind.AnswerDraft;

        if (filled == total && holes == 0 && answers == 0)
            return BoardKind.Answer;

        if (filled + holes == total && answers == holes)
            return BoardKind.Problem;

        return BoardKind.Unknown;
    }

    // -----------------------------
    // 输出（调试）
    // -----------------------------
    public static string ToReadableString(this BoardData board)
    {
        var sb = new StringBuilder();
        int w = board.Layout.Width;
        int h = board.Layout.Height;

        sb.AppendLine($"Board (h({h})×w({w}))");
        sb.Append($"{' ', 6}");
        for (int c = 0; c < w; c++)
            sb.Append($"{c,3} ");
        sb.AppendLine();
        sb.AppendLine(new string('-', w * 4 + 5));

        for (int r = 0; r < h; r++)
        {
            sb.Append($"{r,3} |");
            for (int c = 0; c < w; c++)
            {
                var pos = new RowCol(r, c);
                string repr = "";

                if (board.FilledValues.TryGetValue(pos, out var v))
                    repr = v;
                else if (board.Holes.Contains(pos))
                    repr = board.HoleTypes.TryGetValue(pos, out var t)
                        ? (t == CellType.Number ? "□" : "◇")
                        : "□";
                else if (!board.Layout.IsValid(pos))
                    repr = "";

                sb.Append($"{repr,3} ");
            }
            sb.AppendLine();
        }

        sb.AppendLine(new string('-', w * 4 + 5));

        if (board.PossibleAnswers.Count > 0)
            sb.AppendLine($"PossibleAnswers: {string.Join(", ", board.PossibleAnswers)}");

        return sb.ToString();
    }

    public static void PrettyPrint(this BoardData board)
    {
        Console.WriteLine(board.ToReadableString());
    }

    // -----------------------------
    // 工具方法：排序后的 Holes
    // -----------------------------
    public static List<RowCol> GetSortedHoles(this BoardData board)
    {
        return board.Holes
            .OrderBy(p => p.Row)
            .ThenBy(p => p.Col)
            .ToList();
    }
}
