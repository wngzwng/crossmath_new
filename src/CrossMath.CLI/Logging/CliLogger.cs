using CrossMath.CLI.Framework;

namespace CrossMath.CLI.Logging;

public static class CliLogger
{
    public static void Info(string msg) => ColorConsole.WriteLine(msg, ConsoleColor.Cyan);
    public static void Success(string msg) => ColorConsole.WriteLine(msg, ConsoleColor.Green);
    public static void Warn(string msg) => ColorConsole.WriteLine(msg, ConsoleColor.Yellow);
    public static void Error(string msg) => ColorConsole.WriteLine(msg, ConsoleColor.Red);
}