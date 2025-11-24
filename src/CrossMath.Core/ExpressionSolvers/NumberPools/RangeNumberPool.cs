namespace CrossMath.Core.ExpressionSolvers.NumberPools;

public class RangeNumberPool: INumberPool
{
    private int _min = 0;
    private int _max = 0;

    public RangeNumberPool(int min, int max)
    {
        _min = min;
        _max = max;
    }
    
    public IEnumerable<int> GetAllNumbers()
    {
        for (int i = _min; i <= _max; i++)
        {
            yield return i;
        }
    }

    public void SetRange(int min, int max)
    {
        if (min > max)
        {
            throw new ArgumentException($"min({min}) > max({max})");
        }

        _min = min;
        _max = max;
    }
}