using CrossMath.Core.Models;

namespace CrossMath.Core.Generators.StopPolicies;

public class MaxCountStopPolicy : IStopPolicy
{
    private readonly int _max;

    public MaxCountStopPolicy(int max) => _max = max;

    public bool ShouldStop(int count, LayoutGenContext context, BoardLayout layout)
        => count >= _max;
}
