using CrossMath.Core.Models;

namespace CrossMath.Core.Generators.StopPolicies;

public static class StopPolicyFactory
{
    public static CompositeBuilder CreateComposite()
        => new CompositeBuilder();

    public static MaxCountStopPolicy MaxCount(int count)
        => new MaxCountStopPolicy(count);

    public static TimeoutStopPolicy Timeout(TimeSpan span)
        => new TimeoutStopPolicy(span);

    public static CancellationTokenStopPolicy Canceled(CancellationToken token)
        => new CancellationTokenStopPolicy(token);

    public static CustomStopPolicy Custom(
        Func<int, LayoutGenContext, BoardLayout, bool> fn)
        => new CustomStopPolicy(fn);

    public static CategoryQuotaStopPolicy<TK> CategoryQuota<TK>(
        Func<BoardLayout, TK> categorySelector,
        Dictionary<TK, double> ratioMap,
        int totalCount)
    {
        return new CategoryQuotaStopPolicy<TK>(categorySelector, ratioMap, totalCount);
    }
}