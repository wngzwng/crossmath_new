namespace CrossMath.Core.ExpressionSolvers.NumberPools;

public static class NumberPoolFactory
{
    public static INumberPool Create(int min, int max)
    {
        if (min > max)
        {
            throw new ArgumentException($"min({min}) > max({max})");
        }

        return new RangeNumberPool(min, max);
    }

    public static INumberPool Create(IEnumerable<int> numList)
    {
        return new DiscreteNumberPool(numList);
    }
}