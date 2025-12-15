namespace CrossMath.Core.Utils.Progress;

public interface IProgressWriter
{
    void Write(string text);
    void WriteLine(string text = "");
}