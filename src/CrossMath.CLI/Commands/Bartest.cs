using System.CommandLine;
using CrossMath.CLI.Framework;
using CrossMath.CLI.Utils.Progress;

namespace CrossMath.CLI.Commands;

public sealed class BartestCommand : CliCommandBase
{
    private readonly Option<int> _total =
        new("--total")
        {
            Description = "进度条步数",
            DefaultValueFactory = _ => 100
        };

    private readonly Option<int> _interval =
        new("--interval")
        {
            Description = "刷新间隔（毫秒）",
            DefaultValueFactory = _ => 50,
        };

    public BartestCommand() : base("bartest", "演示进度条") { }

    protected override void Configure(Command cmd)
    {
        cmd.Add(_total);
        cmd.Add(_interval);
    }

    protected override async Task<int> HandleAsync(ParseResult parse)
    {
        int total = parse.GetValue(_total);
        int interval = parse.GetValue(_interval);

        using var bar = new ConsoleProgressReporter(total, "进度条测试", interval);

        for (int i = 0; i < total; i++)
        {
            await Task.Delay(20);
            bar.Report();
        }
        return 0;
    }
}