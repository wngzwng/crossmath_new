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
}
