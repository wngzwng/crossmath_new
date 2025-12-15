using System.Text;
using System.CommandLine;

namespace CrossMath.CLI.Framework.Docs;
public static class DocsGenerator
{
    public static string OutputDir =>
        Path.Combine(Directory.GetCurrentDirectory(), "docs", "cli");

    public static void Generate(
        RootCommand rootCommand,
        string format = "md",
        string outputDir = "docs/cli",
        bool cleanOutput = false,
        bool verbose = false)
    {
        // ---------------- Clean Output Directory ----------------
        if (cleanOutput && Directory.Exists(outputDir))
        {
            if (verbose)
            {
                Console.WriteLine($"[docs] 清空输出目录: {outputDir}");
            }
            Directory.Delete(outputDir, recursive: true);
        }

        // ---------------- Ensure Directory ----------------
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
            if (verbose)
            {
                Console.WriteLine($"[docs] 创建输出目录: {outputDir}");
            }
        }

        // ---------------- Generate Documentation ----------------
        int count = 0;

        foreach (var cmd in EnumerateCommands(rootCommand))
        {
            string filePath = Path.Combine(outputDir, $"{cmd.Name}.md");

            if (verbose)
            {
                Console.WriteLine($"[docs] 生成文档: {cmd.Name} → {filePath}");
            }

            string doc = RenderForFormat(cmd, format);
            File.WriteAllText(filePath, doc);

            count++;
        }

        // ---------------- Summary ----------------
        Console.WriteLine($"📄 CLI 文档已生成：{outputDir}（共 {count} 个命令）");
    }
    
    // ----------------------------------------------------------
    // 统一格式输出层（现在支持 md，将来可以扩展 man / json）
    // ----------------------------------------------------------
    private static string RenderForFormat(Command cmd, string format)
    {
        return format.ToLower() switch
        {
            "md" => CommandDocRenderer.Render(cmd),
            _ => throw new NotSupportedException($"不支持的文档格式: {format}")
        };
    }

    /// <summary>
    /// 递归遍历整个命令树
    /// </summary>
    private static IEnumerable<Command> EnumerateCommands(Command cmd)
    {
        yield return cmd;

        foreach (var child in cmd.Subcommands)
        {
            foreach (var sub in EnumerateCommands(child))
                yield return sub;
        }
    }
}
