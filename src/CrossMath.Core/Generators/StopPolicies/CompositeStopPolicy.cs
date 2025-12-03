using CrossMath.Core.Models;

namespace CrossMath.Core.Generators.StopPolicies;

public class CompositeStopPolicy : IStopPolicy
{
    private readonly List<IStopPolicy> _policies = new();

    public CompositeStopPolicy Add(IStopPolicy p)
    {
        _policies.Add(p);
        return this;
    }

    public bool ShouldStop(int count, LayoutGenContext context, BoardLayout layout)
    {
        foreach (var p in _policies)
            if (p.ShouldStop(count, context, layout))
                return true;

        return false;
    }
}
