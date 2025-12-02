using business.Records;
using CrossMath.Core.Models;

namespace business.Converters;

public static class LayoutRecordFactory
{
    public static LayoutRecord FromBoardLayout(int id, BoardLayout layout)
    {
        return new LayoutRecord
        {
            Id = id,
            LayoutStr = layout.LayoutStr,
            LayoutSize = layout.BoardSize
        };
    }
}
