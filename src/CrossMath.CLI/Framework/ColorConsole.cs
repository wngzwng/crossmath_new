namespace CrossMath.CLI.Framework;

public static class ColorConsole
{
    private static readonly object _lock = new();

    public static void Write(string message, ConsoleColor color)
    {
        lock (_lock)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = old;
        }
    }

    public static void WriteLine(string message, ConsoleColor color)
    {
        lock (_lock)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = old;
        }
    }
}
