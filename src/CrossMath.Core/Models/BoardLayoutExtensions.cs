using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Types;

namespace CrossMath.Core.Models;

public static class BoardLayoutExtensions
{
    // 工厂方法：从多行字符串创建（关卡编辑器最爱用！）
    public static BoardLayout FromLines(params string[] lines)
    {
        if (lines.Length == 0) throw new ArgumentException("Empty layout");
        int height = lines.Length;
        int width = lines[0].Length;
        if (lines.Any(l => l.Length != width))
            throw new ArgumentException("All lines must have same length");

        return new BoardLayout(string.Join("", lines), width, height);
    }

    // 快速打印（调试用）
    public static void Print(this BoardLayout layout)
        => Console.WriteLine(layout);
    
    // <summary>
    /// 默认逻辑：cell == "1" ? validChar : ' '
    /// </summary>
    public static char DefaultCellChar(int row, int col, string cellValue, char validChar)
        => cellValue == "1" ? validChar : ' ';
    
    public static void PrettyPrint(
        this BoardLayout layout,
        char validChar = '@',
        Func<int, int, string, char>? convert = null)
    {
        convert ??= ((r, c, v) => DefaultCellChar(r, c, v, validChar));
        
        int w = layout.Width;
        int h = layout.Height;

        // 1) 打印列号
        Console.Write($"{' ', 6}");
        for (int c = 0; c < w; c++)
            Console.Write($"{c,3} ");
        Console.WriteLine();

        // 2) 顶部横线
        Console.WriteLine(new string('-', 5 + w * 4));

        // 3) 每一行
        for (int r = 0; r < h; r++)
        {
            Console.Write($"  {r,2} |");

            for (int c = 0; c < w; c++)
            {
                string v = layout[r, c];
                char ch = convert(r, c, v);   // 提供 row & col
                // char ch = layout[r, c] == "1" ? validChar : ' ';
                Console.Write($"  {ch} ");
            }

            Console.WriteLine();
        }

        // 4) 底部横线
        Console.WriteLine(new string('-', 5 + w * 4));
    }

    public static void LogicPrettyPrint(this BoardLayout layout, char numberChar = '□', char operatorChar = '◇')
    {
        var explayouts = ExpressionLayoutBuilder.ExtractLayouts(layout, [5, 7]);
        var posToCellType = ExpressionLayoutGraphUtils.BuildPosToCellTypeMap(explayouts);
        
        layout.PrettyPrint(convert: (r, c, val) =>
        {
            if (val != "1") return ' '; //空白部分

            var pos = RowCol.At(r, c);
            if (posToCellType.ContainsKey(pos))
            {
                return posToCellType[pos] switch
                {
                    CellType.Number => numberChar,
                    CellType.Operator => operatorChar,
                    CellType.Equal => '=',
                    _ => 'x'
                };
            }
            return 'x';
        });
    }
    
    public static double Sigma(this BoardLayout layout)
    {
        var blocks = CalcBlockCount(layout);
        return CalcSigma(blocks);
    }
    
    /// <summary>
    /// 计算总体标准差（population standard deviation）
    /// Python 等价：statistics.pstdev()
    /// </summary>
    private static double CalcSigma(List<int> nums)
    {
        if (nums.Count == 0)
            return 0;

        double mean = nums.Average();
        double variance = nums.Sum(x => (x - mean) * (x - mean)) / nums.Count;
        double std = Math.Sqrt(variance);

        // 如果接近整数（误差允许 1e-9）
        double rounded = Math.Round(std);
        if (Math.Abs(std - rounded) < 1e-9)
            return rounded; // 返回整数（double 类型）

        return Math.Round(std, 2); // 保留两位小数
    }

    public static List<int> CalcBlockCount(BoardLayout layout)
    {
        List<int> result = new();
        foreach (var (start, end) in layout.BoardSize.GetQuadrants())
        {
            result.Add(Size.TraverseSection(start, end).Count(layout.IsValid));
        }

        return result;
    }
    
    
}