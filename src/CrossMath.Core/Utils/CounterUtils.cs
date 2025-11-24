namespace CrossMath.Core.Utils;

public static class CounterUtils
{
    /// <summary>
    /// 统计离散序列中各元素的出现次数。
    /// ------------------------------------------------------------
    /// 可用于数字、字符串或任意可哈希类型。
    /// </summary>
    /// <typeparam name="T">元素类型（需可作为字典键使用）。</typeparam>
    /// <param name="values">输入的元素序列。</param>
    /// <returns>
    /// 包含每个元素及其出现次数的字典。
    /// 若 <paramref name="values"/> 为空，则返回空字典。
    /// </returns>
    public static Dictionary<T, int> CountValues<T>(IEnumerable<T> values)
    {
        if (values is null)
            throw new ArgumentNullException(nameof(values));

        var counts = new Dictionary<T, int>();

        foreach (var v in values)
        {
            // 利用 TryGetValue 避免二次哈希查找
            if (counts.TryGetValue(v, out int count))
                counts[v] = count + 1;
            else
                counts[v] = 1;
        }

        return counts;
    }

    /// <summary>
    /// 检查一个组合是否符合原列表的多重集约束
    /// </summary>
    /// <param name="counts">原列表数字计数字典</param>
    /// <param name="combination">组合数组</param>
    /// <returns>组合是否符合多重集约束</returns>
    public static bool IsValidMultiset<T>(Dictionary<T, int> counts, params T[] combination)
    {
        var tempCounts = new Dictionary<T, int>();
        foreach (var x in combination)
            tempCounts[x] = tempCounts.TryGetValue(x, out int t) ? t + 1 : 1;

        foreach (var kv in tempCounts)
        {
            if (!counts.TryGetValue(kv.Key, out int original) || kv.Value > original)
                return false;
        }
        return true;
    }
}