namespace CrossMath.Core.Codec;

public static partial class BoardDataCodec
{
    #region ===== 结构校验工具 =====

    /// <summary>
    /// 校验 layout 长度是否与 width * height 匹配
    /// </summary>
    private static void ValidateLayoutSize(int width, int height, string layout)
    {
        if (layout.Length != width * height)
            throw new ArgumentException(
                $"layout.Length({layout.Length}) != width({width}) * height({height})");
    }

    /// <summary>
    /// 校验 boardTokens 数量是否与 layout 中有效位置（'1'）数量一致
    /// </summary>
    private static void ValidateBoardTokenCount(
        string layout,
        IReadOnlyList<string> boardTokens)
    {
        int expected = layout.Count(c => c == '1');
        if (boardTokens.Count != expected)
            throw new ArgumentException(
                $"boardTokens({boardTokens.Count}) != layout '1' count({expected})");
    }

    #endregion


    #region ===== 十六进制编解码工具（Wire Format） =====

    /// <summary>
    /// 将整数编码为十六进制字符串
    /// hexWidth 表示十六进制位宽（1: 尺寸字段, 2: 数值 / token 字段）
    /// </summary>
    private static string ToHex(int value, int hexWidth = 2)
    {
        if (hexWidth <= 0)
            throw new ArgumentOutOfRangeException(nameof(hexWidth));

        return value.ToString($"x{hexWidth}");
    }

    /// <summary>
    /// 将单个十六进制字符解码为整数（用于 width / height）
    /// </summary>
    private static int ToInt(char hex)
    {
        return Convert.ToInt32(hex.ToString(), 16);
    }

    /// <summary>
    /// 将十六进制字符串解码为整数（用于 token / number）
    /// </summary>
    private static int ToInt(string hex)
    {
        return Convert.ToInt32(hex, 16);
    }

    #endregion
}