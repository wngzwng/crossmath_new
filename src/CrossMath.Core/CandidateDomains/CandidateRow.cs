
namespace CrossMath.Core.CandidateDomains;

/// <summary>
/// 表示一条候选记录（Row）
/// 由一组 slot -> value 映射组成
/// 支持：
/// 1. 获取槽位对应的值
/// 2. 克隆
/// 3. 相等判断（用于表自动去重）
/// </summary>
public class CandidateRow<TSlot, TValue> :
    IEquatable<CandidateRow<TSlot, TValue>>
    where TSlot : notnull
    where TValue : notnull, IEquatable<TValue>
{
    private readonly Dictionary<TSlot, TValue> _map;

    public CandidateRow()
    {
        _map = new();
    }

    public CandidateRow(Dictionary<TSlot, TValue> source)
    {
        _map = new(source);
    }

    /// <summary>
    /// 所有槽位
    /// </summary>
    public IEnumerable<TSlot> Slots => _map.Keys;

    /// <summary>
    /// 获取该行所有值（不保证顺序）
    /// </summary>
    public IEnumerable<TValue> Values => _map.Values;
    
    
    /// <summary>
    /// 获取第 index 条 Row
    /// </summary>
    public TValue this[TSlot slot] => GetValue(slot);

    /// <summary>
    /// 强制获取槽位对应的值
    /// </summary>
    public TValue GetValue(TSlot slot)
    {
        if (!_map.TryGetValue(slot, out var v))
            throw new KeyNotFoundException($"Slot {slot} not found in row");
        return v;
    }

    /// <summary>
    /// 尝试获取槽位值
    /// </summary>
    public bool TryGetValue(TSlot slot, out TValue value)
        => _map.TryGetValue(slot, out value!);

    /// <summary>
    /// 返回一个新的字典副本（读取-friendly）
    /// </summary>
    public Dictionary<TSlot, TValue> ToDictionary()
        => new Dictionary<TSlot, TValue>(_map);

    /// <summary>
    /// 设置槽位值
    /// </summary>
    public void Set(TSlot slot, TValue value)
    {
        _map[slot] = value;
    }

    /// <summary>
    /// 克隆一条完整 Row
    /// </summary>
    public CandidateRow<TSlot, TValue> Clone()
        => new CandidateRow<TSlot, TValue>(
            new Dictionary<TSlot, TValue>(_map));

    public override string ToString()
        => string.Join(", ", _map.Select(kv => $"{kv.Key}:{kv.Value}"));


    // -----------------------------------------------
    // Equality（用于 CandidateTable 自动去重）
    // -----------------------------------------------
    public bool Equals(CandidateRow<TSlot, TValue>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (_map.Count != other._map.Count) return false;

        foreach (var (slot, value) in _map)
        {
            if (!other._map.TryGetValue(slot, out var v2)) return false;
            if (!value.Equals(v2)) return false;
        }

        return true;
    }

    public override bool Equals(object? obj)
        => obj is CandidateRow<TSlot, TValue> row && Equals(row);

    public override int GetHashCode()
    {
        int hash = 17;
        foreach (var (slot, val) in _map.OrderBy(kv => kv.Key.GetHashCode()))
        {
            hash = hash * 31 + slot.GetHashCode();
            hash = hash * 31 + val.GetHashCode();
        }
        return hash;
    }
}
