namespace CrossMath.Service.Utils.Progress;

public sealed class NullProgressWriter : IProgressWriter
{
    public void Write(string text) {}
    public void WriteLine(string text = "") {}
}
