namespace CrossMath.Core.ExpressionSolvers.NumberPools;

public sealed class RangeNumberPool : INumberPool
{
    private int _min;
    private int _max;

    public RangeNumberPool(int min, int max)
    {
        if (min > max)
            throw new ArgumentException($"min({min}) cannot be greater than max({max})");

        _min = min;
        _max = max;
    }

    public void SetRange(int min, int max)
    {
        if (min > max)
            throw new ArgumentException($"min({min}) > max({max})");

        _min = min;
        _max = max;
    }

    public IEnumerable<int> UniqueNumbers => AllNumbers;

    public IEnumerable<int> AllNumbers
    {
        get
        {
            for (int i = _min; i <= _max; i++)
                yield return i;
        }
    }

    public bool Contains(int number) => number >= _min && number <= _max;

    /// <summary>
    /// 关键：每个数字可以被无限次使用
    /// 返回 int.MaxValue 表示“永不枯竭”
    /// </summary>
    public int GetCount(int number) => Contains(number) ? int.MaxValue : 0;

    // 可选：提供一个语义更清晰的扩展属性（非常推荐）
    public bool IsInfinite => true;
}