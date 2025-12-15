using System.CommandLine;

namespace CrossMath.CLI.Framework.Docs;

public static class DocsCommand
{
    public static Command Build(RootCommand root)
    {
        var cmd = new Command("docs", "生成 CLI 文档");

        // -------- Options --------
        // 文档格式（md / man）
        var formatOpt = new Option<string>("--format")
        {
            Description = "文档格式（md 或 man）",
            DefaultValueFactory = _ => "md"
        };

        // 输出目录
        var outputOpt = new Option<string>("--output")
        {
            Description = "输出目录",
            DefaultValueFactory = _ => "docs/cli"
        };

        // 生成前清空输出目录
        var cleanOpt = new Option<bool>("--clean")
        {
            Description = "生成前清空输出目录"
        };

        // 是否输出更多调试信息
        var verboseOpt = new Option<bool>("--verbose")
        {
            Description = "输出更多调试信息"
        };

        cmd.Add(formatOpt);
        cmd.Add(outputOpt);
        cmd.Add(cleanOpt);
        cmd.Add(verboseOpt);

        // -------- Action --------
        cmd.SetAction(parse =>
        {
            var format = parse.GetOrDefault(formatOpt, "md");
            var output = parse.GetOrDefault(outputOpt, "docs/cli");
            bool   clean   = parse.GetValue(cleanOpt);
            bool   verbose = parse.GetValue(verboseOpt);

            if (verbose)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[docs] 开始生成 CLI 文档…");
                Console.ResetColor();
                Console.WriteLine($"  format = {format}");
                Console.WriteLine($"  output = {output}");
                Console.WriteLine($"  clean  = {clean}");
            }

            DocsGenerator.Generate(
                rootCommand: root,
                format: format,
                outputDir: output,
                cleanOutput: clean,
                verbose: verbose
            );

            if (verbose)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[docs] 文档生成完成！");
                Console.ResetColor();
            }

            return Task.CompletedTask;
        });

        return cmd;
    }
    
    public static T GetOrDefault<T>(this ParseResult parse, Option<T> opt, T defaultValue)
        => parse.GetValue(opt) ?? defaultValue;
}