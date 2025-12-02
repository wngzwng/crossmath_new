using business.Records.Maps;
using CsvHelper;

namespace business.Csv;

public static class CsvProfile
{
    public static void Register(CsvContext ctx)
    {
        ctx.RegisterClassMap<LayoutRecordMap>();
        ctx.RegisterClassMap<LayoutFullRecordMap>();
        // ctx.RegisterClassMap<BoardStatsRecordMap>();
        // ...
    }
}
