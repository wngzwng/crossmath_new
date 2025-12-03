using CrossMath.Core.Models;

namespace CrossMath.Core.Generators.StopPolicies;

public class CompositeBuilder
{
    private readonly CompositeStopPolicy _policy = new();

    public CompositeBuilder Add(IStopPolicy p)
    {
        _policy.Add(p);
        return this;
    }

    public CompositeBuilder MaxCount(int count)
    {
        _policy.Add(new MaxCountStopPolicy(count));
        return this;
    }

    public CompositeBuilder Timeout(TimeSpan span)
    {
        _policy.Add(new TimeoutStopPolicy(span));
        return this;
    }

    public CompositeBuilder Canceled(CancellationToken token)
    {
        _policy.Add(new CancellationTokenStopPolicy(token));
        return this;
    }

    public CompositeBuilder Custom(
        Func<int, LayoutGenContext, BoardLayout, bool> fn)
    {
        _policy.Add(new CustomStopPolicy(fn));
        return this;
    }

    public IStopPolicy Build() => _policy;
}
