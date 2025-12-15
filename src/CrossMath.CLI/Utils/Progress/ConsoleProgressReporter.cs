namespace CrossMath.CLI.Utils.Progress;

public sealed class ConsoleProgressReporter : IDisposable
{
    private readonly Tqdm _bar;

    public ConsoleProgressReporter(int total, string desc, int refreshMs = 50)
    {
        _bar = new Tqdm(total, desc, refreshMs);
    }

    public void Report(int step = 1) => _bar.Report(step);

    public void Dispose() => _bar.Dispose();
}