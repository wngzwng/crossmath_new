namespace CrossMath.Core.Types;

public static class SizeExtensions
{
    /// <summary>
    /// 1. Top-Left
    /// 2. Top-Right
    /// 3. Bottom-Left
    /// 4. Bottom-Right
    /// </summary>
    public static IEnumerable<(RowCol Start, RowCol End)> GetQuadrants(this Size size)
    {
        int h = size.Height;
        int w = size.Width;

        int maxH = h - 1;
        int maxW = w - 1;

        // floor midpoint
        int midH = maxH / 2;
        int midW = maxW / 2;

        // ceil midpoint
        int startBottom = (maxH + 1) / 2;
        int startRight  = (maxW + 1) / 2;

        var rowRanges = new (int r0, int r1)[]
        {
            (0,         midH),       // Top
            (startBottom, maxH)      // Bottom
        };

        var colRanges = new (int c0, int c1)[]
        {
            (0,         midW),       // Left
            (startRight, maxW)       // Right
        };

        foreach (var (r0, r1) in rowRanges)
        {
            foreach (var (c0, c1) in colRanges)
            {
                yield return (new RowCol(r0, c0), new RowCol(r1, c1));
            }
        }
    }
    
   
}