namespace CrossMath.Core.Utils.Progress;

public class StdOutProgressWriter : IProgressWriter
{
    public void Write(string text) => Console.Write(text);
    public void WriteLine(string text = "") => Console.WriteLine(text);
}