using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Codec;

/*
1. 路由 
2. 大块的拆分 {height}{width}{boarddata}{answer} 
    检测：数量是否匹配 
    2.1. layout的数量 height*width 
    2.2 boarddata 与 height*weidht * 2 的匹配 
    2.3 answer 与 空格数的匹配 2.3 boarddata 表示0的值 与layout对应位置的0是否符合 
    
3. 点的匹配 棋盘数据 RowCol：string， 答案映射：answer： string 
4. 数据的转换： 
string 16进制 -> 10进制 -> string 
string 大于 fa， 表示符号位， 重新映射为符号位 检查符号位的映射是否有异常 
上述应该分为这个四层结构，每层结构负责自己的任务 同理Encode 也是差不多的
 */
public static partial class BoardDataCodec
{
    #region ===== 路由层 =====

    // encoded = {h}{w}{board}{answer}
    private static BoardData DecodeConcatenated(string encoded, string layout)
    {
        var split = SplitConcatenated(encoded, layout);
        return DecodeCore(
            split.Width,
            split.Height,
            layout,
            split.BoardTokens,
            split.AnswerTokens);
    }

    // encoded = {h}{w}{board}:{answer}
    private static BoardData DecodeSeparated(string encoded, string layout)
    {
        var split = SplitSeparated(encoded, layout);
        return DecodeCore(
            split.Width,
            split.Height,
            layout,
            split.BoardTokens,
            split.AnswerTokens);
    }

    #endregion


    #region ===== Layer 2：协议结构拆分 =====

    private static (
        int Width,
        int Height,
        List<string> BoardTokens,
        List<string> AnswerTokens
    ) SplitConcatenated(string encoded, string layout)
    {
        encoded = encoded.ToLowerInvariant();

        int height = ToInt(encoded[0]);
        int width  = ToInt(encoded[1]);

        ValidateLayoutSize(width, height, layout);

        var allTokens = Chunk(encoded[2..], 2).ToList();

        int boardTokenCount = layout.Count(c => c == '1');
        if (allTokens.Count < boardTokenCount)
            throw new ArgumentException("board token 数量不足");

        var boardTokens  = allTokens.Take(boardTokenCount).ToList();
        var answerTokens = allTokens.Skip(boardTokenCount).ToList();

        if (boardTokens.Count(IsSpaceToken) != answerTokens.Count)
            throw new ArgumentException("空格数与答案数不符合");

        return (width, height, boardTokens, answerTokens);
    }


    private static (
        int Width,
        int Height,
        List<string> BoardTokens,
        List<string> AnswerTokens
    ) SplitSeparated(string encoded, string layout)
    {
        var parts = encoded.Split(BoardAnswerSeparator, 2);
        var boardPart  = parts[0].ToLowerInvariant();
        var answerPart = parts.Length > 1 ? parts[1].ToLowerInvariant() : "";

        int height = ToInt(boardPart[0]);
        int width  = ToInt(boardPart[1]);

        ValidateLayoutSize(width, height, layout);

        var boardTokens  = Chunk(boardPart[2..], 2).ToList();
        var answerTokens = Chunk(answerPart, 2).ToList();

        int boardTokenCount = layout.Count(c => c == '1');
        if (boardTokens.Count != boardTokenCount)
            throw new ArgumentException("board token 数量与layout给定位置数量不符合");

        if (boardTokens.Count(IsSpaceToken) != answerTokens.Count)
            throw new ArgumentException("空格数与答案数不符合");

        return (width, height, boardTokens, answerTokens);
    }

    #endregion


    #region ===== Layer 3：点级匹配 + Board 组装 =====

    private static BoardData DecodeCore(
        int width,
        int height,
        string layout,
        IReadOnlyList<string> boardTokens,
        IReadOnlyList<string> answerTokens)
    {
        var filled    = new Dictionary<RowCol, string>();
        var holeTypes = new Dictionary<RowCol, CellType>();
        var holes     = new HashSet<RowCol>();

        int k = 0;
        for (int i = 0; i < height; i++)
        for (int j = 0; j < width; j++)
        {
            int idx = i * width + j;
            if (layout[idx] != '1') continue;

            DecodeCell(
                boardTokens[k++],
                new RowCol(i, j),
                filled, holeTypes, holes);
        }

        var answers = answerTokens
            .Select(DecodeAnswerValue)
            .ToList();

        return BoardData.Create(
            layout, width, height,
            filled, holeTypes, holes, answers);
    }

    #endregion


    #region ===== Layer 4：token → 语义 =====

    private static void DecodeCell(
        string token,
        RowCol rc,
        Dictionary<RowCol, string> filled,
        Dictionary<RowCol, CellType> holeTypes,
        HashSet<RowCol> holes)
    {
        if (OperatorCodec.TryDecode(token, out var op))
        {
            filled[rc] = op;
            return;
        }

        if (token == OperatorCodec.OpHoleHex)
        {
            holeTypes[rc] = CellType.Operator;
            holes.Add(rc);
            return;
        }

        if (token == OperatorCodec.NumberHoleHex)
        {
            holeTypes[rc] = CellType.Number;
            holes.Add(rc);
            return;
        }

        filled[rc] = DecodeNumberToken(token);
    }


    private static string DecodeAnswerValue(string token)
    {
        if (OperatorCodec.TryDecode(token, out var op))
            return op;

        return DecodeNumberToken(token);
    }


    private static string DecodeNumberToken(string token)
    {
        int number = ToInt(token);
        if (number >= NumberOpenMax)
            throw new ArgumentException($"未知编码: {token}");

        return number.ToString();
    }


    private static bool IsSpaceToken(string token)
    {
        return token == OperatorCodec.NumberHoleHex
            || token == OperatorCodec.OpHoleHex;
    }

    #endregion


    #region ===== 工具方法 =====

    private static IEnumerable<string> Chunk(string s, int size)
    {
        for (int i = 0; i + size <= s.Length; i += size)
            yield return s.Substring(i, size);
    }

    #endregion
}