namespace CrossMath.Core.Types;

public static class DirectionExtensions
{
    public static (int dr, int dc) ToDelta(this Direction dir)
        => dir == Direction.Horizontal ? (0, 1) : (1, 0);

    public static RowCol ToRowColDelta(this Direction dir)
        => dir == Direction.Horizontal ? RowCol.Right : RowCol.Down;

    public static Direction Perpendicular(this Direction dir)
        => dir == Direction.Horizontal ? Direction.Vertical : Direction.Horizontal;

    public static bool IsHorizontal(this Direction dir) => dir == Direction.Horizontal;
    public static bool IsVertical(this Direction dir)   => dir == Direction.Vertical;

    public static IEnumerable<Direction> All()
    {
        yield return Direction.Horizontal;
        yield return Direction.Vertical;
    }
}