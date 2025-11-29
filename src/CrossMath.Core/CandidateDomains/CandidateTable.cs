namespace CrossMath.Core.CandidateDomains;

/// <summary>
/// 候选域表（Candidate Domain Table）
/// 包含多条候选记录（Row）
///
/// 功能：
/// 1. 自动去重
/// 2. Filter 过滤
/// 3. Project（保留槽位）
/// 4. RemapSlots（槽位重映射）
/// 5. Domain (slot -> 值域)
/// 6. IsEmpty / IsSingle
/// </summary>
public class CandidateTable<TSlot, TValue>
    where TSlot : notnull
    where TValue : notnull, IEquatable<TValue>
{
    private readonly HashSet<CandidateRow<TSlot, TValue>> _set = new();

    public List<CandidateRow<TSlot, TValue>> Rows { get; } = new();

    // =============================================================
    // 基础行为
    // =============================================================

    /// <summary>
    /// 添加一行（自动去重）
    /// </summary>
    public void AddRow(CandidateRow<TSlot, TValue> row)
    {
        if (_set.Add(row))
            Rows.Add(row);
    }

    /// <summary>
    /// 清空
    /// </summary>
    public void Clear()
    {
        Rows.Clear();
        _set.Clear();
    }

    public int Count => Rows.Count;
    public bool IsEmpty => Rows.Count == 0;
    public bool IsSingle => Rows.Count == 1;

    /// <summary>
    /// 获取第 index 条 Row
    /// </summary>
    public CandidateRow<TSlot, TValue> this[int index] => Rows[index];
    
    public IEnumerable<TSlot> Slots => Rows
        .SelectMany(row => row.Slots)
        .Distinct();

    // =============================================================
    // Domain（slot -> 候选值集合）
    // =============================================================
    public Dictionary<TSlot, HashSet<TValue>> Domain
    {
        get
        {
            var domain = new Dictionary<TSlot, HashSet<TValue>>();

            foreach (var row in Rows)
            {
                foreach (var slot in row.Slots)
                {
                    row.TryGetValue(slot, out var v);

                    if (!domain.TryGetValue(slot, out var set))
                        domain[slot] = set = new HashSet<TValue>();

                    set.Add(v);
                }
            }

            return domain;
        }
    }

    // =============================================================
    // 过滤
    // =============================================================
    public CandidateTable<TSlot, TValue> Where(
        Func<CandidateRow<TSlot, TValue>, bool> predicate)
    {
        var t = new CandidateTable<TSlot, TValue>();
        foreach (var row in Rows)
            if (predicate(row))
                t.AddRow(row.Clone());
        return t;
    }

    // =============================================================
    // 只保留某些槽位
    // =============================================================
    public CandidateTable<TSlot, TValue> Project(params TSlot[] keepSlots)
    {
        var keep = keepSlots.ToHashSet();

        var t = new CandidateTable<TSlot, TValue>();

        foreach (var row in Rows)
        {
            var newRow = new CandidateRow<TSlot, TValue>();

            foreach (var slot in keep)
            {
                if (row.TryGetValue(slot, out var v))
                    newRow.Set(slot, v);
            }

            t.AddRow(newRow);
        }

        return t;
    }

    // =============================================================
    // 槽位重映射（如 RowCol -> int）
    // =============================================================
    public CandidateTable<TSlotNew, TValue> RemapSlots<TSlotNew>(
        Func<TSlot, TSlotNew> mapper)
        where TSlotNew : notnull
    {
        var t = new CandidateTable<TSlotNew, TValue>();

        foreach (var row in Rows)
        {
            var newRow = new CandidateRow<TSlotNew, TValue>();

            foreach (var slot in row.Slots)
            {
                row.TryGetValue(slot, out var v);
                newRow.Set(mapper(slot), v);
            }

            t.AddRow(newRow);
        }

        return t;
    }

    public CandidateTable<TSlot, TValue> Clone()
    {
        var t = new CandidateTable<TSlot, TValue>();
        foreach (var row in Rows)
            t.AddRow(row.Clone());
        return t;
    }
}
