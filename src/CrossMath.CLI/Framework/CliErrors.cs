using CrossMath.CLI.Logging;

namespace CrossMath.CLI.Framework;

public static class CliErrors
{
    public static int Error(string msg, int code = 1)
    {
        CliLogger.Error(msg);
        Environment.ExitCode = code;
        return code;
    }

    public static int Exception(Exception ex, int code = 1)
    {
        CliLogger.Error("发生未处理异常：");
        CliLogger.Error(ex.Message);
        CliLogger.Error(ex.StackTrace ?? "");
        Environment.ExitCode = code;
        return code;
    }
}