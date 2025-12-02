namespace business.works;

using CrossMath.Core.Types;

public static class LayoutFormulaSpec
{
    public static readonly Dictionary<Size, (int Min, int Max)> Specs =
        new()
        {
            [new Size(Height: 7, Width: 5)] = (4, 7),
            [new Size(Height: 7, Width: 7)] = (4, 8),
            [new Size(Height: 9, Width: 7)] = (5, 9),
            [new Size(Height: 9, Width: 9)] = (5, 10),
            [new Size(Height: 11, Width: 9)] = (7, 15),
            [new Size(Height: 11, Width: 11)] = (8, 21),
        };
}
