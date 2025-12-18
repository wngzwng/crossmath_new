using System.CommandLine;

namespace CrossMath.CLI;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var root = new RootCommand("CrossMath CLI 工具集");
        // 注册子命令
        root.Add(Framework.Docs.DocsCommand.Build(root));
        root.Add(new Commands.BartestCommand().Build());
        root.Add(new Commands.HoleCommand().Build());
        root.Add(new DiagramDirective());
        return await root.Parse(args).InvokeAsync();
    }
}

/**
find  /Users/admin/RiderProjects/Puzzle/CrossMath/data/split -name '*.csv' -print0 \
| parallel -0 -j 8 \
    --joblog hole.log \
    --halt soon,fail=1 \
    ./CrossMath.CLI  hole -i {} --output-dir /Users/admin/RiderProjects/Puzzle/CrossMath/data/split_result

*/