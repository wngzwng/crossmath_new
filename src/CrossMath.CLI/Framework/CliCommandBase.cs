using System.CommandLine;
using CrossMath.CLI.Logging;

namespace CrossMath.CLI.Framework;

public abstract class CliCommandBase
{
    protected readonly Command Command;

    public CliCommandBase(string name, string? description = null)
    {
        Command = new Command(name, description);
        Configure(Command);
        Bind(Command);
    }

    protected abstract void Configure(Command command);
    protected abstract Task<int> HandleAsync(ParseResult parse);

    private void Bind(Command cmd)
    {
        cmd.SetAction(async parse =>
        {
            try
            {
                CliLogger.Info($"执行命令：{cmd.Name}");
                var code = await HandleAsync(parse);

                if (code == 0)
                    CliLogger.Success($"{cmd.Name} 执行成功");
                else
                    CliErrors.Error($"{cmd.Name} 执行失败（退出码 {code}）", code);
            }
            catch (Exception ex)
            {
                CliErrors.Exception(ex);
            }
        });
    }

    public Command Build() => Command;
}