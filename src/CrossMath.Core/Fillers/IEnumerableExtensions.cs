namespace CrossMath.Core.Fillers;

public static class IEnumerableExtensions
{
    private static readonly Random Rnd = Random.Shared; // .NET 6+ 推荐的线程安全实例

    // 方式1：最常用、最快（适用于 List、数组等）
    public static T RandomElementOrDefault<T>(this IEnumerable<T> source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        if (source is IList<T> list)
        {
            if (list.Count == 0) return default;
            return list[Rnd.Next(list.Count)];
        }

        if (source is IReadOnlyList<T> readOnlyList)
        {
            if (readOnlyList.Count == 0) return default;
            return readOnlyList[Rnd.Next(readOnlyList.Count)];
        }

        // 降级到通用方式（会遍历两次）
        var lst = source.ToList();
        return lst.Count == 0 ? default : lst[Rnd.Next(lst.Count)];
    }

    // 方式2：蓄水池算法（Reservoir Sampling），只遍历一次，适用于任何 IEnumerable<T>
    public static T RandomElementByReservoirSampling<T>(this IEnumerable<T> source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        T result = default;
        int count = 0;

        foreach (var item in source)
        {
            count++;
            // 第 count 个元素有 1/count 的概率替换当前结果
            if (Rnd.Next(count) == 0)   // Next(count) 返回 [0, count)
                result = item;
        }

        if (count == 0) return default;
        return result;
    }

    // 方式3：洗牌后取第一个（最均匀，但需要全部加载到内存）
    public static T RandomElementByShuffle<T>(this IEnumerable<T> source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        var array = source.ToArray();
        if (array.Length == 0) return default;

        // Fisher-Yates 现代洗牌
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Rnd.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }

        return array[0];
    }
    
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        var array = source.ToArray();
        if (array.Length == 0) return default;

        // Fisher-Yates 现代洗牌
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Rnd.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
        return array;
    }
}