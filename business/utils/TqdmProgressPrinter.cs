using System.Diagnostics;

namespace business.utils;

public sealed class TqdmProgressPrinter
{
    private readonly Stopwatch _sw = Stopwatch.StartNew();
    private readonly int _barWidth;
    private int _lastPrintedLength = 0;

    private readonly string _description;

    /// <summary>
    /// 创建进度条。
    /// </summary>
    /// <param name="description">描述信息，将显示在进度条前面。例如： "Layouts" 或 "Generating".</param>
    /// <param name="barWidth">进度条宽度。</param>
    public TqdmProgressPrinter(string? description = null, int barWidth = 30)
    {
        _description = description ?? string.Empty;
        _barWidth = barWidth;
    }

    /// <summary>
    /// 报告进度。与 tqdm 一致：Report(current, total)
    /// </summary>
    public void Report(int current, int total)
    {
        if (total <= 0) return;

        double progress = current / (double)total;
        int filled = (int)(_barWidth * progress);
        int empty = _barWidth - filled;

        double elapsed = _sw.Elapsed.TotalSeconds;
        double itPerSec = current / Math.Max(elapsed, 1e-9);
        double eta = (total - current) / Math.Max(itPerSec, 1e-9);

        string bar = new string('█', filled) + new string('░', empty);

        string prefix = string.IsNullOrEmpty(_description) ? "" : $"{_description}: ";

        string msg =
            $"{prefix}{progress:0%}|{bar}| {current}/{total} " +
            $"[{FormatTime(elapsed)}<{FormatTime(eta)}, {itPerSec:0.0}it/s]";

        // 清除上一行
        Console.Write('\r' + new string(' ', _lastPrintedLength) + '\r');

        Console.Write(msg);

        _lastPrintedLength = msg.Length;

        if (current >= total)
            Console.WriteLine();
    }

    private static string FormatTime(double sec)
    {
        if (sec < 60)
            return $"{sec:00}s";
        if (sec < 3600)
            return $"{sec / 60:00}m{sec % 60:00}s";
        return $"{sec / 3600:00}h{sec % 3600 / 60:00}m";
    }
}