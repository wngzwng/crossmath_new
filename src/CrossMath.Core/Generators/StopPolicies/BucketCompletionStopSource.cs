using CrossMath.Core.Generators.Collectors;
using CrossMath.Core.Models;

namespace CrossMath.Core.Generators.StopPolicies;

public sealed class BucketCompletionStopPolicy<TKey> : IStopPolicy where TKey : notnull
{
    private readonly BucketCounter<TKey> _collector;
    
    public BucketCompletionStopPolicy(BucketCounter<TKey> counter) => _collector = counter;
    
    public bool ShouldStop(int count, LayoutGenContext context, BoardLayout layout)
    {
        return _collector.IsAllCompleted;
    }
}