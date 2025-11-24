namespace CrossMath.Core.ExpressionSolvers.NumberPools;

public class DiscreteNumberPool:INumberPool
{
    private IReadOnlyList<int> _numList;

    public DiscreteNumberPool(IEnumerable<int> numList)
    {
        _numList = numList.Distinct().ToList();
    }
    
    public IEnumerable<int> GetAllNumbers()
    {
        return _numList;
    }

    public void SetSource(IEnumerable<int> numList)
    {
        _numList = numList.Distinct().ToList();
    }
}