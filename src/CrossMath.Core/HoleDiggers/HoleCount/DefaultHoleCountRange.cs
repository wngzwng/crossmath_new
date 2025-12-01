using CrossMath.Core.Types;

namespace CrossMath.Core.HoleDiggers.HoleCount;

public static class DefaultHoleCountRange
{
    private static readonly Dictionary<Size, (int minHoleCount, int maxHoleCount)> holeCountRanges;

    static DefaultHoleCountRange()
    {
        holeCountRanges = new Dictionary<Size, (int minHoleCount, int maxHoleCount)>
        {
            { new Size(7, 7), (24, 40) },  // 9×9盘面，最小挖空数24，最大挖空数40
            // 可以根据需要添加其他盘面大小的配置
            // { 6, (16, 26) }, // 6×6盘面
            // { 4, (10, 16) }  // 4×4盘面
        };
    }

    /// <summary>
    /// 获取指定盘面大小对应的默认挖空数量范围
    /// </summary>
    public static (int minHoleCount, int maxHoleCount) GetHoleCountRange(Size size)
    {
        if (holeCountRanges.TryGetValue(size, out var range))
        {
            return range;
        }
        
        throw new ArgumentException($"不支持的盘面大小: {size}。支持的盘面大小为: {string.Join(", ", holeCountRanges.Keys)}");
    }

    /// <summary>
    /// 获取所有支持的盘面大小
    /// </summary>
    public static IEnumerable<Size> SupportedSizes => holeCountRanges.Keys;

    /// <summary>
    /// 检查指定的盘面大小是否在支持的范围内
    /// </summary>
    public static bool IsSizeSupported(Size size)
    {
        return holeCountRanges.ContainsKey(size);
    }
}