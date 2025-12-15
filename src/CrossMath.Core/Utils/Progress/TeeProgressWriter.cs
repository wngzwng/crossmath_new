namespace CrossMath.Core.Utils.Progress;

public sealed class TeeProgressWriter : IProgressWriter
{
    private readonly IReadOnlyList<IProgressWriter> _writers;

    public TeeProgressWriter(params IProgressWriter[] writers)
    {
        _writers = writers;
    }

    public void Write(string text)
    {
        foreach (var w in _writers)
            w.Write(text);
    }

    public void WriteLine(string text = "")
    {
        foreach (var w in _writers)
            w.WriteLine(text);
    }
}
