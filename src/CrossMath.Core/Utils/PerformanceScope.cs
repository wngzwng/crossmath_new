using System.Diagnostics;

namespace CrossMath.Core.Utils;

public sealed class PerformanceScope : IDisposable
{
    private readonly Stopwatch _stopwatch;
    private readonly string _operationName;

    public PerformanceScope(string operationName)
    {
        _operationName = operationName;
        _stopwatch = Stopwatch.StartNew();
    }

    public static PerformanceScope Measure(string operationName)
    {
        return new PerformanceScope(operationName);
    }
    public void Dispose()
    {
        _stopwatch.Stop();
        Console.WriteLine($"{_operationName} 执行耗时：{_stopwatch.ElapsedMilliseconds}ms");
    }

    public long GetElapsedMilliseconds()
    {
        return _stopwatch.ElapsedMilliseconds;
    }
}

// // 使用时可以获取耗时
// using var scope = PerformanceMeasurement.Measure("排序并连接字符串");
// var result = testData.OrderBy(s => s).ToArray();
// var hashString = string.Join(",", result);
//
// long elapsedMs = scope.GetElapsedMilliseconds(); // 获取耗时