using System.Diagnostics;

namespace CrossMath.Core.Utils.Progress;


public sealed class Tqdm : IDisposable
{
    private readonly int _total;
    private readonly Stopwatch _sw = new();
    private int _current = 0;
    private readonly int _refreshIntervalMs;
    private long _lastRefresh = 0;
    private readonly string? _prefix;

    private readonly IProgressWriter _writer;

    public Tqdm(
        int total, 
        string? desc = null, 
        int refreshIntervalMs = 50,
        IProgressWriter? writer = null)
    {
        _total = total;
        _prefix = desc;
        _refreshIntervalMs = refreshIntervalMs;
        _writer = writer ?? new StdOutProgressWriter(); // 默认 stdout

        _sw.Start();
        Render();
    }
    

    public void Update(int step = 1)
    {
        Interlocked.Add(ref _current, step);

        var now = _sw.ElapsedMilliseconds;

        if (now - _lastRefresh >= _refreshIntervalMs || _current == _total)
        {
            _lastRefresh = now;
            Render();
        }
    }

    private void Render()
    {
        double progress = (double)_current / _total;
        const int barWidth = 30;

        int filled = (int)(progress * barWidth);
        string bar = new string('#', filled) + new string('-', barWidth - filled);
        
        string color = AnsiColor.GetColor(progress);
        string coloredBar = $"{color}{bar}{AnsiColor.Reset}";
        
        var elapsed = _sw.Elapsed;
        TimeSpan eta = TimeSpan.Zero;

        if (_current > 0)
        {
            double rate = elapsed.TotalSeconds / _current;
            eta = TimeSpan.FromSeconds((_total - _current) * rate);
        }

        string prefix = string.IsNullOrWhiteSpace(_prefix) ? "" : $"{_prefix} ";
        
        _writer.Write(
            $"\r{prefix}[{coloredBar}] {_current}/{_total} {progress * 100:0.0}% | " +
            $"Elapsed: {elapsed:mm\\:ss} | ETA: {eta:mm\\:ss}"
        );
    }

    public void Dispose()
    {
        Render();
        _writer.WriteLine();
    }
}
