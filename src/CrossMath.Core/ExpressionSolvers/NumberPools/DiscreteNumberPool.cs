using CrossMath.Core.Types;

namespace CrossMath.Core.ExpressionSolvers.NumberPools;

public sealed class DiscreteNumberPool : INumberPool
{
    private IReadOnlyList<int> _source;              // 原始多重集：允许重复
    private Dictionary<int, int>? _countCache;       // 缓存计数

    /// <summary>
    /// 排序方式：影响 UniqueNumbers，AllNumbers 保留原始顺序
    /// </summary>
    public NumberOrder Order { get; set; } = NumberOrder.Ascending;

    private static readonly Random _rng = new();

    public DiscreteNumberPool(IEnumerable<int> source, NumberOrder order = NumberOrder.Ascending)
    {
        _source = source?.ToArray() ?? throw new ArgumentNullException(nameof(source));
        Order = order;
    }

    public void SetSource(IEnumerable<int> source)
    {
        _source = source?.ToArray() ?? throw new ArgumentNullException(nameof(source));
        _countCache = null;
    }

    // ==========================================================
    // 1. UniqueNumbers（带排序）
    // ==========================================================
    public IEnumerable<int> UniqueNumbers
    {
        get
        {
            var list = _source.Distinct().ToList();

            switch (Order)
            {
                case NumberOrder.Ascending:
                    list.Sort();
                    break;

                case NumberOrder.Descending:
                    list.Sort();
                    list.Reverse();
                    break;

                case NumberOrder.Shuffled:
                    // Fisher–Yates
                    for (int i = list.Count - 1; i > 0; i--)
                    {
                        int j = _rng.Next(i + 1);
                        (list[i], list[j]) = (list[j], list[i]);
                    }
                    break;
            }

            return list;
        }
    }

    // ==========================================================
    // 2. AllNumbers（保持原始顺序！）
    // ==========================================================
    public IEnumerable<int> AllNumbers => _source;

    // ==========================================================
    // 3. 查询
    // ==========================================================
    public bool Contains(int number) => _source.Contains(number);

    public int GetCount(int number)
    {
        _countCache ??= _source
            .GroupBy(x => x)
            .ToDictionary(g => g.Key, g => g.Count());

        return _countCache.TryGetValue(number, out var count) ? count : 0;
    }

    public bool IsInfinite => false;
}
