using CrossMath.Core.Models;

namespace CrossMath.Core.Generators.StopPolicies;

public class CategoryQuotaStopPolicy<TK> : IStopPolicy
    where TK : notnull
{
    private readonly Func<BoardLayout, TK> _categorySelector;

    private readonly Dictionary<TK, int> _expected;
    private readonly Dictionary<TK, int> _collected;

    public CategoryQuotaStopPolicy(
        Func<BoardLayout, TK> categorySelector,
        Dictionary<TK, double> ratioMap,
        int totalCount)
    {
        _categorySelector = categorySelector;

        // 按比例生成期待数量
        _expected = ratioMap.ToDictionary(
            kv => kv.Key,
            kv => (int)(kv.Value * totalCount));

        // 收集计数器
        _collected = ratioMap.Keys.ToDictionary(k => k, _ => 0);
    }

    // public bool ShouldStop(int count, LayoutGenContext context, BoardLayout layout)
    // {
    //     TK key = _categorySelector(layout);
    //
    //     // 若不在期待类别中 → 拒绝
    //     if (!_expected.ContainsKey(key))
    //         return true;
    //
    //     int collected = _collected[key];
    //     int expected = _expected[key];
    //
    //     // 若该类已满 → 拒绝
    //     if (collected >= expected)
    //         return true;
    //
    //     // 接受并计数
    //     _collected[key]++;
    //     return false;
    // }
    //
    public bool ShouldStop(int count, LayoutGenContext context, BoardLayout layout)
    {
        TK key = _categorySelector(layout);

        // 若不在期待类别中 → 拒绝
        if (!_expected.ContainsKey(key))
            return true;

        int collected = _collected[key];
        int expected = _expected[key];

        // 若该类已满 → 拒绝
        if (collected >= expected)
            return true;

        // 接受并计数
        _collected[key]++;
        return false;
    }
}
