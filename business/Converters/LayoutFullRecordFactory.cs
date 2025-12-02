using business.Records;
using CrossMath.Core.Analytics.EmptyBoard;
using CrossMath.Core.Models;

namespace business.Converters;

public class LayoutFullRecordFactory
{
    public static LayoutFullRecord FromBoardLayout(int id, BoardLayout layout)
    {
        return new LayoutFullRecord
        {
            Id = id,
            LayoutInfo = layout.LayoutStr,
            Size = layout.BoardSize
        };
    }
    
    public static LayoutFullRecord FromLayoutRecord(LayoutRecord record)
    {
        var id = record.Id;
        var boardLayout = new BoardLayout(record.LayoutStr, record.LayoutSize);
        var fullRecord = EmptyBoardAnalyzer.GetInfo(boardLayout).ToFullRecord();
        fullRecord.Id = id;
        return fullRecord;
    }
}