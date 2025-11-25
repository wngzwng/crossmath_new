using CrossMath.Core.Types;

namespace CrossMath.Core.ExpressionSolvers.NumberPools;

public static class NumberPoolFactory
{
    public static INumberPool Create(int min, int max, NumberOrder order)
    {
        if (min > max)
        {
            throw new ArgumentException($"min({min}) > max({max})");
        }

        return new RangeNumberPool(min, max, order);
    }

    public static INumberPool Create(IEnumerable<int> numList)
    {
        return new DiscreteNumberPool(numList);
    }
}