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
    
    // 美观打印（表格格式）
    public static void PrettyPrint(this BoardLayout layout, char validChar = '@')
    {
        int w = layout.Width;
        int h = layout.Height;

        // 1) 打印列号
        Console.Write("     ");
        for (int c = 0; c < w; c++)
            Console.Write($"{c,3} ");
        Console.WriteLine();

        // 2) 顶部横线
        Console.WriteLine(new string('-', 5 + w * 4));

        // 3) 每一行
        for (int r = 0; r < h; r++)
        {
            Console.Write($"  {r} |");

            for (int c = 0; c < w; c++)
            {
                char ch = layout[r, c] == "1" ? validChar : ' ';
                Console.Write($"  {ch} ");
            }

            Console.WriteLine();
        }

        // 4) 底部横线
        Console.WriteLine(new string('-', 5 + w * 4));
    }
}