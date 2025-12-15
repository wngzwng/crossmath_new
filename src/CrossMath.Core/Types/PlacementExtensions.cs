namespace CrossMath.Core.Types;

public static class PlacementExtensions
{
    public static IEnumerable<(int R, int C)> EnumerateCells(this Placement p)
    {
        for (int i = 0; i < p.Length; i++)
        {
            yield return p.Direction == Direction.Horizontal
                ? (p.Row, p.Col + i)
                : (p.Row + i, p.Col);
        }
    }
    
    public static IEnumerable<RowCol> EnumerateCellsAsRowCol(this Placement p)
    {
        foreach (var (r, c) in p.EnumerateCells())
        {
            yield return RowCol.At(r, c);
        }
    }

    public static bool Contains(this Placement p, int r, int c)
    {
        return p.Direction switch
        {
            Direction.Horizontal => r == p.Row && c >= p.Col && c < p.Col + p.Length,
            Direction.Vertical   => c == p.Col && r >= p.Row && r < p.Row + p.Length,
            _ => false
        };
    }
    
    public static bool Contains(this Placement p, RowCol pos)
    {
        return p.Direction switch
        {
            Direction.Horizontal => pos.Row == p.Row && pos.Col >= p.Col && pos.Col < p.Col + p.Length,
            Direction.Vertical   => pos.Col == p.Col && pos.Row >= p.Row && pos.Row < p.Row + p.Length,
            _ => false
        };
    }
    
    public static bool OverlapsWithSameDirection(this Placement p, Placement other)
    {
        /*
           这才是经典的“一维线段重叠判定”公式！
           数学证明：两区间 [a, a+l) 和 [b, b+m) 重叠 ⇔ max(a,b) < min(a+l, b+m)
         */
        // 强制要求同方向（调用方保证），省一次分支
        return p.Direction == Direction.Horizontal
            ? (p.Row == other.Row && Math.Max(p.Col, other.Col) < Math.Min(p.Col + p.Length, other.Col + other.Length))
            : (p.Col == other.Col && Math.Max(p.Row, other.Row) < Math.Min(p.Row + p.Length, other.Row + other.Length));
    }
}