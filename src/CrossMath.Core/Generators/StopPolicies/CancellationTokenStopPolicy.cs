using CrossMath.Core.Models;

namespace CrossMath.Core.Generators.StopPolicies;

public class CancellationTokenStopPolicy : IStopPolicy
{
    private readonly CancellationToken _token;

    public CancellationTokenStopPolicy(CancellationToken token)
    {
        _token = token;
    }

    public bool ShouldStop(int count, LayoutGenContext context, BoardLayout layout)
        => _token.IsCancellationRequested;
}
