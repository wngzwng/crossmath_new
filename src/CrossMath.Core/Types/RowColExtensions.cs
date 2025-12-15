namespace CrossMath.Core.Types;

public static class RowColExtensions
{
    public static RowCol Up(this RowCol p) => p + RowCol.Up; 
    public static RowCol Down(this RowCol p) => p + RowCol.Down; 
    public static RowCol Left(this RowCol p) => p + RowCol.Left; 
    public static RowCol Right(this RowCol p) => p + RowCol.Right;

    public static IEnumerable<RowCol> GetFourNeighbors(this RowCol p) 
    { 
        yield return p.Up(); 
        yield return p.Down(); 
        yield return p.Left(); 
        yield return p.Right(); 
    }
    
    /// <summary>
    /// 计算一组点的轴对齐最小包围矩形（左上角和右下角）
    /// </summary>
    public static (RowCol Min, RowCol Max) GetMinMaxPosition(this IEnumerable<RowCol> positions)
    {
        if (positions == null) throw new ArgumentNullException(nameof(positions));

        using var e = positions.GetEnumerator();
        if (!e.MoveNext())
            return (RowCol.Zero, RowCol.Zero); // 或自定义 Empty

        var first = e.Current;
        int minR = first.Row, maxR = first.Row;
        int minC = first.Col, maxC = first.Col;

        while (e.MoveNext())
        {
            var p = e.Current;
            if (p.Row < minR) minR = p.Row;
            if (p.Row > maxR) maxR = p.Row;
            if (p.Col < minC) minC = p.Col;
            if (p.Col > maxC) maxC = p.Col;
        }

        return (new RowCol(minR, minC), new RowCol(maxR, maxC));
    }
}
