using System.Diagnostics;
using CrossMath.Core.Models;

namespace CrossMath.Core.Generators.StopPolicies;

public class TimeoutStopPolicy : IStopPolicy
{
    private readonly Stopwatch _sw = Stopwatch.StartNew();
    private readonly TimeSpan _limit;

    public TimeoutStopPolicy(TimeSpan limit) => _limit = limit;

    public bool ShouldStop(int count, LayoutGenContext context, BoardLayout layout)
        => _sw.Elapsed >= _limit;
}
