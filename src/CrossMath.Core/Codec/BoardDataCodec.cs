using System.Text;
using CrossMath.Core.Types;
using CrossMath.Core.Models;

namespace CrossMath.Core.Codec;


public static partial class BoardDataCodec
{
    #region ===== 协议常量（Public）=====

    /// <summary>
    /// Board 数据与 Answer 数据之间的分隔符。
    /// 协议形式：{h}{w}{board}[:{answer}]
    /// </summary>
    public const string BoardAnswerSeparator = ":";

    /// <summary>
    /// 数字编码的最大开区间上界（十六进制）。
    /// 0x00 ~ 0xF9 为合法数字编码，0xFA 及以上为操作符号区。
    /// </summary>
    public static readonly int NumberOpenMax = ToInt("fa");

    #endregion


    #region ===== Encode =====

    /// <summary>
    /// 将 BoardData 编码为十六进制字符串。
    /// 返回 (encoded, layout)，用于 Decode 对称还原。
    /// </summary>
    public static (string encoded, string layout) Encode(BoardData board)
    {
        if (board == null)
            throw new ArgumentNullException(nameof(board));

        return Encode(board, BoardAnswerSeparator);
    }

    #endregion


    #region ===== Decode（Wire Format → BoardData）=====

    /// <summary>
    /// 解码十六进制编码与 layout，自动识别是否包含 Answer 分隔符。
    /// </summary>
    public static BoardData Decode(string encoded, string layout)
    {
        if (string.IsNullOrWhiteSpace(encoded))
            throw new ArgumentException("encoded 不能为空", nameof(encoded));

        if (string.IsNullOrWhiteSpace(layout))
            throw new ArgumentException("layout 不能为空", nameof(layout));

        return encoded.Contains(BoardAnswerSeparator)
            ? DecodeSeparated(encoded, layout)
            : DecodeConcatenated(encoded, layout);
    }

    #endregion


    #region ===== Decode（Layout → 空 BoardData）=====

    /// <summary>
    /// 根据 layout 与尺寸构造一个空的 BoardData（不包含编码数据）。
    /// </summary>
    public static BoardData Decode(string layout, int width, int height)
    {
        return DecodeLayoutCore(layout, width, height);
    }

    /// <summary>
    /// 根据 layout 与 Size 构造一个空的 BoardData。
    /// </summary>
    public static BoardData Decode(string layout, Size size)
    {
        return DecodeLayoutCore(layout, size.Width, size.Height);
    }

    /// <summary>
    /// 根据 BoardLayout 构造一个空的 BoardData。
    /// </summary>
    public static BoardData Decode(BoardLayout boardLayout)
    {
        if (boardLayout == null)
            throw new ArgumentNullException(nameof(boardLayout));

        return DecodeLayoutCore(
            boardLayout.LayoutStr,
            boardLayout.Width,
            boardLayout.Height);
    }

    #endregion
    
    private static BoardData DecodeLayoutCore(
        string layout,
        int width,
        int height)
    {
        if (string.IsNullOrWhiteSpace(layout))
            throw new ArgumentException("布局不能为空");

        ValidateLayoutSize(width, height, layout);

        if (layout.Any(c => c is not ('0' or '1')))
            throw new ArgumentException("布局必须只包含 0 或 1");

        var boardLayout = new BoardLayout(layout, width, height);
        return new BoardData(boardLayout);
    }
}