using business.Records.Columns;

namespace business.Records.Maps;

using CsvHelper.Configuration;
using business.Converters;

public sealed class LayoutFullRecordMap : ClassMap<LayoutFullRecord>
{
    public LayoutFullRecordMap()
    {
        Map(m => m.Id).Name(LayoutFullRecordColumns.Id);
        
        Map(m => m.Size)
            .Name(LayoutFullRecordColumns.Size)
            .TypeConverter(SizeConverter.Create('x'));

        Map(m => m.LayoutInfo).Name(LayoutFullRecordColumns.LayoutInfo);

        Map(m => m.FormulaCount).Name(LayoutFullRecordColumns.FormulaCount);
        Map(m => m.Formula7Count).Name(LayoutFullRecordColumns.Formula7Count);
        Map(m => m.RingCount).Name(LayoutFullRecordColumns.RingCount);
        Map(m => m.Sigma).Name(LayoutFullRecordColumns.Sigma);
        Map(m => m.CrossCount).Name(LayoutFullRecordColumns.CrossCount);
        Map(m => m.FormulaCoverage).Name(LayoutFullRecordColumns.FormulaCoverage);
        Map(m => m.FormulaZCount).Name(LayoutFullRecordColumns.FormulaZCount);

        Map(m => m.OutermostTop).Name(LayoutFullRecordColumns.OutermostTop);
        Map(m => m.OutermostBottom).Name(LayoutFullRecordColumns.OutermostBottom);
        Map(m => m.OutermostLeft).Name(LayoutFullRecordColumns.OutermostLeft);
        Map(m => m.OutermostRight).Name(LayoutFullRecordColumns.OutermostRight);
        Map(m => m.SigmaOutermost).Name(LayoutFullRecordColumns.SigmaOutermost);

        Map(m => m.OuterTop).Name(LayoutFullRecordColumns.OuterTop);
        Map(m => m.OuterBottom).Name(LayoutFullRecordColumns.OuterBottom);
        Map(m => m.OuterLeft).Name(LayoutFullRecordColumns.OuterLeft);
        Map(m => m.OuterRight).Name(LayoutFullRecordColumns.OuterRight);
    }
}
