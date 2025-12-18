namespace CrossMath.Service.Utils.Progress;

public interface IProgressWriter
{
    void Write(string text);
    void WriteLine(string text = "");
}