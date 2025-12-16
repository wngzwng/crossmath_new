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
        root.Add(new DiagramDirective());
        return await root.Parse(args).InvokeAsync();
    }
}