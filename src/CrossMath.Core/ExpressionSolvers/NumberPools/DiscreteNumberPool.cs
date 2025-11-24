namespace CrossMath.Core.ExpressionSolvers.NumberPools;

public sealed class DiscreteNumberPool : INumberPool
{
    private IReadOnlyList<int> _source;        // 原始数据，保留重复！
    private Dictionary<int, int>? _countCache; // 可选：缓存加速 GetCount

    public DiscreteNumberPool(IEnumerable<int> source)
    {
        _source = source?.ToArray() ?? throw new ArgumentNullException(nameof(source));
        // 不去重！保留所有重复数字
    }

    public void SetSource(IEnumerable<int> source)
    {
        _source = source?.ToArray() ?? throw new ArgumentNullException(nameof(source));
        _countCache = null; // 失效缓存
    }

    public IEnumerable<int> UniqueNumbers => _source.Distinct();
    public IEnumerable<int> AllNumbers => _source;

    public bool Contains(int number) => _source.Contains(number);

    public int GetCount(int number)
    {
        // 性能优化：第一次用时缓存
        _countCache ??= _source.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        return _countCache.TryGetValue(number, out var count) ? count : 0;
    }

    public bool IsInfinite => false;
}