using CrossMath.Core.Models;

namespace CrossMath.Core.Generators.StopPolicies;

public class CustomStopPolicy : IStopPolicy
{
    private readonly Func<int, LayoutGenContext, BoardLayout, bool> _func;
    public CustomStopPolicy(Func<int, LayoutGenContext, BoardLayout, bool> func) 
        => _func = func;

    public bool ShouldStop(int count, LayoutGenContext context, BoardLayout layout)
        => _func(count, context, layout);
}
