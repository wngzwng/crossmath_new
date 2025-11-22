using CrossMath.Core.Types;
namespace CrossMath.Core.Expressions.Layout;

public static class ExpressionLayoutBuilderCore
{
    public static List<ExpressionLayout> ExtractLayouts(
        Size size,
        Func<RowCol, bool> isValid,
        IEnumerable<int> allowExpressionLengths)
    {
        return ExtractLayouts(size.Height, size.Width, isValid, allowExpressionLengths);
    }
    
    public static List<ExpressionLayout> ExtractLayouts(
        int height,
        int width,
        Func<RowCol, bool> isValid,
        IEnumerable<int> allowExpressionLengths)
    {
        var results = new List<ExpressionLayout>();
        var lengths = allowExpressionLengths.Distinct().OrderBy(x => x).ToArray();

        foreach (var pos in IterAllPositions(height, width))
        {
            if (!isValid(pos)) continue;

            foreach (var dir in new[] { Direction.Horizontal, Direction.Vertical })
            {
                if (!IsExpressionStart(pos, dir, isValid))
                    continue;

                foreach (int len in lengths)
                {
                    if (TryExtract(pos, dir, len, isValid, out var coords))
                    {
                        ExpressionId id = ExpressionId.New();
                        results.Add(new ExpressionLayout(id, dir, coords));
                    }
                }
            }
        }

        return results;
    }

    private static IEnumerable<RowCol> IterAllPositions(int h, int w)
    {
        for (int r = 0; r < h; r++)
            for (int c = 0; c < w; c++)
                yield return new RowCol(r, c);
    }

    private static bool IsExpressionStart(RowCol pos, Direction dir, Func<RowCol, bool> isValid)
    {
        var (dr, dc) = dir.ToDelta();
        var prev = new RowCol(pos.Row - dr, pos.Col - dc);
        var next = new RowCol(pos.Row + dr, pos.Col + dc);

        return !isValid(prev) && isValid(next);
    }

    private static bool TryExtract(RowCol start, Direction dir, int len, Func<RowCol, bool> isValid, out IReadOnlyList<RowCol> coords)
    {
        coords = Array.Empty<RowCol>();
        var res = new List<RowCol>(len);
        var (dr, dc) = dir.ToDelta();

        for (int i = 0; i < len; i++)
        {
            var p = new RowCol(start.Row + dr * i, start.Col + dc * i);
            if (!isValid(p)) return false;
            res.Add(p);
        }

        coords = res;
        return true;
    }
}
