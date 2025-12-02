using business.Records;
using CrossMath.Core.Analytics.EmptyBoard;

namespace business.Converters;

public static class EmptyBoardConverters
{
    /// <summary>
    /// EmptyBoardBrief → LayoutFullRecord
    /// 属性完全一致时自动映射。
    /// </summary>
    public static LayoutFullRecord ToFullRecord(this EmptyBoardBrief src)
        => CopyProperties<EmptyBoardBrief, LayoutFullRecord>(src);

    /// <summary>
    /// LayoutFullRecord → EmptyBoardBrief
    /// 属性完全一致时自动映射。
    /// </summary>
    public static EmptyBoardBrief ToBrief(this LayoutFullRecord src)
        => CopyProperties<LayoutFullRecord, EmptyBoardBrief>(src);

    private static TDest CopyProperties<TSource, TDest>(TSource source)
        where TDest : new()
    {
        var dest = new TDest();

        var sProps = typeof(TSource).GetProperties();
        var dProps = typeof(TDest).GetProperties()
            .ToDictionary(x => x.Name);

        foreach (var s in sProps)
        {
            if (!dProps.TryGetValue(s.Name, out var d)) continue;
            if (!d.CanWrite) continue;

            d.SetValue(dest!, s.GetValue(source));
        }

        return dest;
    }
}
