using System.Diagnostics;

namespace CrossMath.CLI.Utils.Progress;

public sealed class Tqdm : IDisposable
{
    private readonly int _total;
    private int _current;
    private readonly int _refresh;
    private readonly Stopwatch _sw = new();

    public Tqdm(int total, string? desc = null, int refreshMs = 50)
    {
        _total = total;
        Desc = desc;
        _refresh = refreshMs;
        _sw.Start();
        Render(force: true);
    }

    public string? Desc { get; }

    public void Report(int step = 1)
    {
        Interlocked.Add(ref _current, step);
        Render(force: true);
    }

    private void Render(bool force = false)
    {
        int cur = Math.Min(_current, _total);
        double p = cur / (double)_total;
        string bar = new string('#', (int)(p * 30)).PadRight(30, '-');

        Console.Write(
            $"\r{Desc} [{bar}] {cur}/{_total} {p * 100:0.0}%"
        );
    }

    public void Dispose()
    {
        Render(force: true);
        Console.WriteLine();
    }
}
