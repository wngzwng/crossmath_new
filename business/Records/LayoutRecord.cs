using CrossMath.Core.Types;

namespace business.Records;

public record LayoutRecord
{
    public int Id { get; set; }
    public string LayoutStr { get; set; }
    public Size LayoutSize { get; set; }
}