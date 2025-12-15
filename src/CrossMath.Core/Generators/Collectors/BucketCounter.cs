using System.Collections.Concurrent;
using CrossMath.Core.Models;

namespace CrossMath.Core.Generators.Collectors;

/// <summary>
/// 纯粹的分桶计数器（只计数，不存对象）
/// 职责：累计每个桶的数量 + 判断是否全部达到配额
/// </summary>
public sealed class BucketCounter<TKey> where TKey : notnull
{
    private readonly Func<BoardLayout, TKey> _selector;

    /// <summary>每个类别的目标数量（配额）</summary>
    public IReadOnlyDictionary<TKey, int> Quota { get; }

    /// <summary>当前已计数数量（线程安全）</summary>
    private readonly Dictionary<TKey, int> _counts;
    
    /// <summary>
    /// 记录已经达到 quota 的 key（防止重复触发事件）
    /// </summary>
    private readonly HashSet<TKey> _completedKeys = new();

    /// <summary>
    /// 某个桶第一次被填满时触发 (key, quota)
    /// </summary>
    public event Action<TKey, int>? BucketCompleted;
    
    /// <summary>
    /// 全部桶达标（只触发一次）
    /// </summary>
    public event Action? AllCompleted;
    
    private bool _allCompletedTriggered = false;
    public BucketCounter(
        Func<BoardLayout, TKey> categorySelector,
        IReadOnlyDictionary<TKey, double> ratioMap,
        int totalTargetCount)
    {
        if (ratioMap.Sum(kv => kv.Value) < 0.999)
            throw new ArgumentException("比例和必须接近 1.0", nameof(ratioMap));

        _selector = categorySelector;

        // 使用 Round 保证总和最接近目标数量
        var quota = new Dictionary<TKey, int>();
        foreach (var kv in ratioMap)
        {
            quota[kv.Key] = (int)Math.Round(kv.Value * totalTargetCount);
        }

        // 修正舍入误差（可选：保证总和 == totalTargetCount）
        int currentSum = quota.Values.Sum();
        int diff = totalTargetCount - currentSum;
        if (diff != 0)
        {
            // 简单策略：把误差加到数量最多的桶上
            var biggest = quota.OrderByDescending(kv => kv.Value).First();
            quota[biggest.Key] += diff;
        }

        Quota = quota;
        _counts = new Dictionary<TKey, int>(
            quota.Keys.ToDictionary(k => k, _ => 0));
    }

    /// <summary>
    /// 尝试为一个布局计数 +1
    /// 只有属于配额内且该桶未满时才会真正 +1
    /// </summary>
    /// <returns>true=成功计数（被接受），false=被拒绝（不在配额或已满）</returns>
    public bool TryIncrement(BoardLayout layout)
    {
        TKey key = _selector(layout);

        // 1. 不在配额内 → 直接拒绝
        if (!Quota.TryGetValue(key, out int quota))
            return false;

        // 2. 当前计数
        int current = _counts.GetValueOrDefault(key, 0);

        // 3. 已满 → 拒绝计数
        if (current >= quota)
            return false;

        // 4. 增加计数
        current++;
        _counts[key] = current;

        // 5. ★ 达到 quota，触发桶满事件
        if (current == quota)
        {
            lock (_completedKeys)
            {
                if (_completedKeys.Add(key))
                {
                    BucketCompleted?.Invoke(key, quota);
                }
            }
        }
        
        if (!_allCompletedTriggered && IsAllCompleted)
        {
            _allCompletedTriggered = true;
            AllCompleted?.Invoke();
        }

        return true;
    }

    /// <summary>
    /// 【核心】所有桶是否都已达标？这就是收集侧的“停止信号源”
    /// </summary>
    public bool IsAllCompleted
        => _counts.Count == Quota.Count && // 防止遗漏
           _counts.All(kv => kv.Value >= Quota[kv.Key]);

    /// <summary>
    /// 当前已收集总数
    /// </summary>
    public int TotalCollected => _counts.Values.Sum();

    /// <summary>
    /// 目标总数
    /// </summary>
    public int TotalQuota => Quota.Values.Sum();

    /// <summary>
    /// 当前每个桶的计数（只读快照）
    /// </summary>
    public IReadOnlyDictionary<TKey, int> CurrentCounts => _counts;

    /// <summary>
    /// 美观的进度字符串（控制台友好）
    /// </summary>
    public string ProgressString
    {
        get
        {
            var parts = Quota.Select(kv =>
            {
                int current = _counts.GetValueOrDefault(kv.Key, 0);
                bool done = current >= kv.Value;
                return done
                    ? $"[Success]{kv.Key}: {current}/{kv.Value}"
                    : $"{kv.Key}: {current}/{kv.Value}";
            });
            return string.Join("  │  ", parts);
        }
    }

    /// <summary>
    /// 进度百分比（0~100）
    /// </summary>
    public double CompletionRate
        => TotalQuota == 0 ? 100.0 : Math.Round(100.0 * TotalCollected / TotalQuota, 2);
}