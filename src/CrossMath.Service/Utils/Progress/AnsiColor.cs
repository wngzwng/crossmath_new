namespace CrossMath.Service.Utils.Progress;

public static class AnsiColor
{
    public const string Reset = "\u001b[0m";
    public const string Red = "\u001b[31m";
    public const string Yellow = "\u001b[33m";
    public const string Green = "\u001b[32m";

    public static string GetColor(double progress)
    {
        if (progress < 0.4) return Red;
        if (progress < 0.7) return Yellow;
        return Green;
    }
}
