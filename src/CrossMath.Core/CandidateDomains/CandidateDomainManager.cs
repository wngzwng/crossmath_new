using System;
using System.Collections.Generic;
using System.Linq;
using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Utils;

namespace CrossMath.Core.CandidateDomains;

/// <summary>
/// 管理多个 ExpressionLayout 对应的候选域表（Candidate Domain Table）
///
/// 功能：
/// 1. 新建/添加/获取 table
/// 2. 更新表（槽位被确定）
/// 3. 全局剪枝（槽位 domain 变小）
/// </summary>
public class CandidateDomainManager<TSlot, TValue>
    where TSlot : notnull
    where TValue : notnull, IEquatable<TValue>
{
    private readonly Dictionary<ExpressionLayout, CandidateTable<TSlot, TValue>> _map
        = new();
    
    private readonly Dictionary<string, ExpressionLayout> _idToLayout = new();
    
    // slot -> list of layouts
    private readonly Dictionary<TSlot, HashSet<ExpressionLayout>> _slotIndex
        = new();

    // =============================================================
    // 表管理
    // =============================================================

    public CandidateTable<TSlot, TValue> Create(ExpressionLayout layout)
    {
        var t = new CandidateTable<TSlot, TValue>();
        _map[layout] = t;
        return t;
    }

    public void Add(ExpressionLayout layout, CandidateTable<TSlot, TValue> table)
    {
        _map[layout] = table;
        _idToLayout[layout.Id.Value] = layout;

        foreach (var row in table.Rows)
        {
            foreach (var slot in row.Slots)
            {
                if (!_slotIndex.TryGetValue(slot, out var set))
                    _slotIndex[slot] = set = new();

                set.Add(layout);
            }
        }
    }

    public void Remove(ExpressionLayout layout)
    {
        _map.Remove(layout);
        _idToLayout.Remove(layout.Id.Value);
    }

    public void Remove(string layoutid)
    {
        if (!_idToLayout.TryGetValue(layoutid, out var layout)) return;
        Remove(layout);
    }
    
    

    public CandidateTable<TSlot, TValue> Get(ExpressionLayout layout)
        => _map[layout];
    
    public CandidateTable<TSlot, TValue> Get(string layoutid) => _map[_idToLayout[layoutid]];

    public ExpressionLayout GetLayout(string layoutid) => _idToLayout[layoutid];
    public bool Contains(ExpressionLayout layout)
        => _map.ContainsKey(layout);

    // =============================================================
    // 更新：某槽位被确定（slot -> confirmedValue）
    // =============================================================
    /// <summary>
    /// 某槽位被确定（slot -> confirmedValue）
    /// removeThisCol = true 时：
    ///     在候选表中移除这个槽位列
    /// </summary>
    public void UpdateSlotValueConfirmed(
        TSlot slot,
        TValue confirmedValue,
        bool removeThisCol = false)
    {
        // 精准更新
        // 1. 获取受影响的 layout（真正精准更新）
        if (!_slotIndex.TryGetValue(slot, out var affectedLayouts))
            return; // 没有任何表达式包含这个 slot
        
        foreach (var layout  in affectedLayouts.ToList())
        {
            var table = _map[layout];
            // 1. 保留 slot-value 匹配的 row
            var filtered = table.Where(row =>
            {
                if (!row.TryGetValue(slot, out var v)) return false;
                return v.Equals(confirmedValue);
            });

            // 2. 是否移除该列（slot）
            if (removeThisCol && filtered.Rows.Count > 0)
            {
                var slotsToKeep = filtered
                    .Rows[0]
                    .Slots
                    .Where(s => !s.Equals(slot))
                    .ToArray();

                filtered = filtered.Project(slotsToKeep);
            }

            _map[layout] = filtered;
        }
        
        // 3. 更新 slotIndex（此槽位已经填确定值，若列移除，应删索引）
        if (removeThisCol)
        {
            _slotIndex.Remove(slot);
        }
    }

    // =============================================================
    // 更新：某槽位候选 domain 缩小
    // =============================================================
    public void PruneByDomain(List<TValue> newDomain)
    {
        var counter = CounterUtils.CountValues(newDomain);

        foreach (var (layout, table) in _map.ToList())
        {
            var filtered = table.Where(row =>
            {
                var rowValues = row.Values.ToArray();
                return CounterUtils.IsValidMultiset(counter, rowValues);
            });

            _map[layout] = filtered;
        }
    }

    // =============================================================
    // 遍历
    // =============================================================
    public IEnumerable<ExpressionLayout> Layouts => _map.Keys;
    public IEnumerable<CandidateTable<TSlot, TValue>> Tables => _map.Values;


    public CandidateDomainManager<TSlot, TValue> Clone()
    {
        return new CandidateDomainManager<TSlot, TValue>();
    }
}
