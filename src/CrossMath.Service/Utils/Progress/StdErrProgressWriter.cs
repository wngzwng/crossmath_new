namespace CrossMath.Service.Utils.Progress;
public class StdErrProgressWriter : IProgressWriter
{
    public void Write(string text) => Console.Error.Write(text);
    public void WriteLine(string text = "") => Console.Error.WriteLine(text);
}