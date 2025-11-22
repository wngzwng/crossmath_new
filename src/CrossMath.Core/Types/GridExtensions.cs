namespace CrossMath.Core.Types;

public static class GridExtensions
{
    // 坐标 + 方向 + 步长 → 新坐标（最常用！）
    public static RowCol Offset(this RowCol pos, Direction dir, int steps = 1)
        => pos + dir.ToRowColDelta() * steps;

    // 从起点按方向行走 N 步（生成算式格子序列）
    public static IEnumerable<RowCol> Walk(this RowCol start, Direction dir, int length)
    {
        var delta = dir.ToRowColDelta();
        for (int i = 0; i < length; i++)
            yield return start + delta * i;
    }

    // 判断某个点是否在矩形范围内
    public static bool InBounds(this RowCol pos, Size size) => size.Contains(pos);

    // 生成矩形内所有坐标（关卡遍历必备）
    public static IEnumerable<RowCol> AllPositions(this Size size)
    {
        for (int r = 0; r < size.Height; r++)
        for (int c = 0; c < size.Width; c++)
            yield return new RowCol(r, c);
    }
} 