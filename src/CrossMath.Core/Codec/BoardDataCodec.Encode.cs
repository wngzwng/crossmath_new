using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Codec;

public static partial class BoardDataCodec
{
    #region ===== Public API (Layer 1) =====

    /// <summary>
    /// 编码 BoardData 为紧凑十六进制字符串。
    /// 形式：{h}{w}{board}[{separator}{answer}]
    /// </summary>
    public static (string boardHex, string layout) Encode(
        BoardData b,
        string boardAnswerSeparator)
    {
        ValidateLayoutSize(b.Width, b.Height, b.Layout.LayoutStr);

        var (boardTokens, answerTokens) = GenerateTokens(b);

        var fullHex =
            ToHex(b.Height, 1) +
            ToHex(b.Width, 1) +
            string.Concat(boardTokens);

        if (answerTokens.Count > 0)
        {
            fullHex += boardAnswerSeparator + string.Concat(answerTokens);
        }

        return (fullHex, b.Layout.LayoutStr);
    }

    #endregion


    #region ===== Token Generation (Layer 2 & 3) =====

    /// <summary>
    /// 依据 ValidPositions（行优先）生成 boardTokens，
    /// answerTokens 保持 PossibleAnswers 的顺序。
    /// </summary>
    private static (List<string> boardTokens, List<string> answerTokens)
        GenerateTokens(BoardData b)
    {
        var validPositions = b.ValidPositions.ToList();
        var boardTokens = new List<string>(validPositions.Count);

        foreach (var rc in validPositions)
        {
            boardTokens.Add(EncodeBoardToken(b, rc));
        }

        var answerTokens = b.PossibleAnswers
            .Select(EncodeValueToken)
            .ToList();

        return (boardTokens, answerTokens);
    }

    #endregion


    #region ===== Token Encoding (Layer 4) =====

    /// <summary>
    /// 根据棋盘位置生成 board token（填充值 / 孔位）
    /// </summary>
    private static string EncodeBoardToken(BoardData b, RowCol rc)
    {
        if (b.IsFilled(rc))
        {
            return EncodeValueToken(b[rc]);
        }

        if (b.HoleTypes.TryGetValue(rc, out var cellType))
        {
            return cellType == CellType.Operator
                ? OperatorCodec.OpHoleHex
                : OperatorCodec.NumberHoleHex;
        }

        throw new InvalidOperationException(
            $"位置 {rc} 既无填充值也未标记为孔");
    }

    /// <summary>
    /// 将值（数字或运算符）编码为两位十六进制 token
    /// </summary>
    private static string EncodeValueToken(string value)
    {
        if (OperatorCodec.TryEncode(value, out var opHex))
            return opHex;

        if (int.TryParse(value, out int num))
        {
            if (num >= NumberOpenMax)
                throw new ArgumentException(
                    $"数字值 {num} 超出可编码范围（最大 {NumberOpenMax - 1}）");

            return ToHex(num, 2);
        }

        throw new ArgumentException($"无法编码的值: '{value}'");
    }

    #endregion
}