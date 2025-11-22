namespace CrossMath.Core.ExpressionSolvers;

public class NumberPool
{
    public int Min;
    public int Max;
    
    public IReadOnlyList<int> NumList { get; private set; }

    public NumberPool(int min, int max)
    {
        Min = min;
        Max = max;
        NumList = Enumerable.Range(Min, Max - Min + 1).ToList();
    }

    public NumberPool(IEnumerable<int> numList)
    {
        NumList = numList.ToList();
        Min = NumList.Min();
        Max = NumList.Max();
    }
    
    public bool Contains(int num)
    {
        return NumList.Contains(num);
    }
}