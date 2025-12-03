using CrossMath.Core.Models;

namespace CrossMath.Core.Generators.StopPolicies;

public interface IStopPolicy
{
    bool ShouldStop(int count, LayoutGenContext context, BoardLayout layout);
}
