using CrossMath.Core.Types;

namespace CrossMath.Core.ExpressionSolvers.NumberPools;

public sealed class RangeNumberPool : INumberPool
{
    private int _min;
    private int _max;

    public NumberOrder Order { get; set; } = NumberOrder.Ascending;

    private static readonly Random _rng = new();

    public RangeNumberPool(int min, int max, NumberOrder order = NumberOrder.Ascending)
    {
        if (min > max)
            throw new ArgumentException($"min({min}) cannot be greater than max({max})");

        _min = min;
        _max = max;
        Order = order;
    }

    public void SetRange(int min, int max)
    {
        if (min > max)
            throw new ArgumentException($"min({min}) > max({max})");
        _min = min;
        _max = max;
    }

    public IEnumerable<int> UniqueNumbers => EnumerateOrdered(_min, _max);

    public IEnumerable<int> AllNumbers => UniqueNumbers; // 无限池，次数无限

    public bool Contains(int number) => number >= _min && number <= _max;

    public int GetCount(int number) =>
        Contains(number) ? int.MaxValue : 0;

    public bool IsInfinite => true;


    // ==========================================================
    // 统一排序逻辑
    // ==========================================================
    private IEnumerable<int> EnumerateOrdered(int min, int max)
    {
        switch (Order)
        {
            case NumberOrder.Ascending:
                for (int i = min; i <= max; i++)
                    yield return i;
                break;

            case NumberOrder.Descending:
                for (int i = max; i >= min; i--)
                    yield return i;
                break;

            case NumberOrder.Shuffled:
                // Fisher–Yates shuffle
                var list = Enumerable.Range(min, max - min + 1).ToList();
                for (int i = list.Count - 1; i > 0; i--)
                {
                    int j = _rng.Next(i + 1);
                    (list[i], list[j]) = (list[j], list[i]);
                }
                foreach (var n in list)
                    yield return n;
                break;
        }
    }
}
