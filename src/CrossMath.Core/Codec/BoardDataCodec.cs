using System.Text;
using CrossMath.Core.Types;
using CrossMath.Core.Models;

namespace CrossMath.Core.Codec;

public static class BoardDataCodec
{
    // 初级盘中，答案与原数据使用 : 作为分隔  
    
    /// <summary>编码 BoardData 为 (hex串, layout串)</summary>
    public static (string boardHex, string layout) Encode(BoardData b)
    {
        if (b.Width <= 0 || b.Height <= 0)
            throw new ArgumentException("棋盘尺寸无效");
        if (string.IsNullOrEmpty(b.Layout.LayoutStr))
            throw new ArgumentException("布局不能为空");

        var sb = new StringBuilder();
        sb.Append($"{b.Height:x1}{b.Width:x1}");

        for (int i = 0; i < b.Height; i++)
        {
            for (int j = 0; j < b.Width; j++)
            {
                int idx = i * b.Width + j;
                if (idx >= b.Layout.Length) break;
                if (b.Layout[idx] != "1") continue;

                var rc = new RowCol(i, j);
                if (b.FilledValues.TryGetValue(rc, out var token))
                {
                    if (OperatorCodec.IsOperatorToken(token))
                    {
                        if (!OperatorCodec.TryEncode(token, out var hex))
                            throw new InvalidOperationException($"未知运算符: {token}");
                        sb.Append(hex);
                    }
                    else
                    {
                        if (!int.TryParse(token, out var n))
                            throw new InvalidOperationException($"非法数字: {token}");
                        sb.Append($"{n:x2}");
                    }
                }
                else if (b.HoleTypes.TryGetValue(rc, out var t))
                {
                    sb.Append(t == CellType.Operator
                        ? OperatorCodec.OpHoleHex
                        : OperatorCodec.NumberHoleHex);
                }
                else
                {
                    throw new InvalidOperationException($"有效格 {rc} 未指定内容或空格类型");
                }
            }
        }

        // 附加候选项
        foreach (var s in b.PossibleAnswers)
        {
            if (OperatorCodec.IsOperatorToken(s))
            {
                if (!OperatorCodec.TryEncode(s, out var hex))
                    throw new InvalidOperationException($"未知候选运算符: {s}");
                sb.Append(hex);
            }
            else
            {
                if (!int.TryParse(s, out var n))
                    throw new InvalidOperationException($"非法候选数字: {s}");
                sb.Append($"{n:x2}");
            }
        }
        
        return (sb.ToString(), b.Layout.LayoutStr);
    }
    
    
    /// <summary>解码为 BoardData</summary>
    public static BoardData Decode(string hex, string layout)
    {
        if (string.IsNullOrWhiteSpace(hex))
            throw new ArgumentException("编码不能为空");
        if (string.IsNullOrWhiteSpace(layout))
            throw new ArgumentException("布局不能为空");

        hex = hex.ToLowerInvariant();

        if (hex.Length < 2)
            throw new ArgumentException("编码过短");
        if (hex.Length % 2 != 0)
            throw new ArgumentException("编码长度必须为偶数");
        if (layout.Any(c => c is not ('0' or '1')))
            throw new ArgumentException("布局必须只包含 0 或 1");

        // ---------- 尺寸解析 ----------
        int h = Convert.ToInt32(hex[0].ToString(), 16);
        int w = Convert.ToInt32(hex[1].ToString(), 16);
        if (h <= 0 || w <= 0)
            throw new ArgumentException($"无效尺寸: h:{h} × w:{w}");
        if (layout.Length != w * h)
            throw new ArgumentException($"布局尺寸不对: layout.Length({layout.Length}) !== {w * h}(w:{w} * h:{h}");


        var cellMap = new Dictionary<RowCol, string>();
        var holeTypes = new Dictionary<RowCol, CellType>();
        var holePositions = new HashSet<RowCol>();
        var candidates = new List<string>();
        int dataIndex = 2;

        // ---------- 内部工具函数 ----------
        string? DecodeToken(string token, out CellType? holeType)
        {
            holeType = null;

            if (OperatorCodec.TryDecode(token, out var sym))
                return sym;
            
            if (token == OperatorCodec.OpHoleHex)
            {
                holeType = CellType.Operator;
                return null;
            }

            if (token == OperatorCodec.NumberHoleHex)
            {
                holeType = CellType.Number;
                return null;
            }

            int num = Convert.ToInt32(token, 16);
            if (num <= Convert.ToInt32("fa", 16))
                return num.ToString();

            throw new ArgumentException($"未知编码: {token}");
        }

        // ---------- 棋盘主体解析 ----------
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                int idx = i * w + j;
                if (idx >= layout.Length) break;
                if (layout[idx] != '1') continue;

                if (dataIndex + 2 > hex.Length)
                    throw new ArgumentException("数据不完整");

                var token = hex.Substring(dataIndex, 2);
                dataIndex += 2;

                var rc = new RowCol(i, j);
                var value = DecodeToken(token, out var holeType);

                if (holeType.HasValue)
                {
                    holeTypes[rc] = holeType.Value;
                    holePositions.Add(rc);
                }
                else if (value != null)
                    cellMap[rc] = value;
            }
        }

        // ---------- 候选解析 ----------
        while (dataIndex + 2 <= hex.Length)
        {
            var token = hex.Substring(dataIndex, 2);
            dataIndex += 2;

            var value = DecodeToken(token, out _);
            if (value != null)
                candidates.Add(value);
        }

        // ---------- 结果组装 ----------
        return BoardData.Create(
            layoutStr: layout,
            width: w,
            height: h,
            filledValues: cellMap,
            holeTypes: holeTypes,
            holes: holePositions,
            possibleAnswers: candidates);
    }


    public static BoardData Decode(string layout, int width, int height)
    {
        if (string.IsNullOrWhiteSpace(layout))
            throw new ArgumentException("布局不能为空");

        if (layout.Length != width * height)
            throw new ArgumentException(
                $"布局尺寸不对: layout.Length({layout.Length}) !== {width * height}(w:{width} * h:{height}");
        
        if (layout.Any(c => c is not ('0' or '1')))
            throw new ArgumentException("布局必须只包含 0 或 1");
        
        // ---------- 结果组装 ----------
        var boardLayout = new BoardLayout(layout: layout, width: width, height: height);
        return new BoardData(boardLayout);
    }

    public static BoardData Decode(string layout, Size size)
    {
        return Decode(layout, size.Width, size.Height);
    }

    public static BoardData Decode(BoardLayout boardLayout)
    {
        return Decode(boardLayout.LayoutStr, boardLayout.Width, boardLayout.Height);
    }
}
    