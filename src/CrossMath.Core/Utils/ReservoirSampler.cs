
namespace CrossMath.Core.Utils;

/// <summary>
/// 提供蓄水池抽样（Reservoir Sampling）算法的通用实现。
/// 可从任意 IEnumerable 序列中以均匀概率随机选取一个元素。
/// </summary>
public static class ReservoirSampler
{
    /// <summary>
    /// 从给定序列中随机选取一个元素（等概率）。
    /// 如果序列为空，则返回 false。
    /// </summary>
    /// <typeparam name="T">序列元素类型</typeparam>
    /// <param name="source">输入序列</param>
    /// <param name="random">随机数生成器</param>
    /// <param name="result">输出选中的元素</param>
    /// <returns>若选中元素则返回 true，否则 false</returns>
    public static bool TrySampleOne<T>(IEnumerable<T> source, Random random, out T result)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (random == null)
            throw new ArgumentNullException(nameof(random));

        result = default!;
        int count = 0;

        foreach (var item in source)
        {
            count++;
            // 以 1/count 的概率替换
            if (random.Next(count) == 0)
                result = item;
        }

        return count > 0;
    }
}