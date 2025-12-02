using business.Converters;
using business.Records.Columns;
using CsvHelper.Configuration;

namespace business.Records.Maps;

public sealed class LayoutRecordMap : ClassMap<LayoutRecord>
{
    public LayoutRecordMap()
    {
        Map(m => m.Id).Name(LayoutRecordColumns.Id);

        Map(m => m.LayoutStr).Name(LayoutRecordColumns.Layout);

        Map(m => m.LayoutSize)
            .Name(LayoutRecordColumns.Size)
            .TypeConverter(SizeConverter.Create('x'));
    }
}
