using CrossMath.Core.Utils;

namespace CrossMath.Core.CandidateDomains
{
    public class CandidateDomainManager<TSlot, TValue>
        where TSlot : notnull
        where TValue : notnull, IEquatable<TValue>
    {
        private readonly Dictionary<string, CandidateTable<TSlot, TValue>> _map = new();
        private readonly Dictionary<TSlot, HashSet<string>> _slotToTables = new();

        // =============================================================
        // 表管理
        // =============================================================

        public CandidateTable<TSlot, TValue> Create(string tableName)
        {
            var t = new CandidateTable<TSlot, TValue>();
            _map[tableName] = t;
            return t;
        }

        public void Add(string tableName, CandidateTable<TSlot, TValue> table)
        {
            _map[tableName] = table;
            RegisterTableSlots(tableName, table);
        }

        private void RegisterSlotToTableMapping(TSlot slot, string tableName)
        {
            if (!_slotToTables.ContainsKey(slot))
            {
                _slotToTables[slot] = new HashSet<string>();
            }

            _slotToTables[slot].Add(tableName);
        }

        private void RegisterTableSlots(string tableName, CandidateTable<TSlot, TValue> table)
        {
            foreach (var tableSlot in table.Slots)
            {
                RegisterSlotToTableMapping(tableSlot, tableName);
            }
        }

        public void Remove(string tableName)
        {
            _map.Remove(tableName);
        }

        public CandidateTable<TSlot, TValue> Get(string tableName) => _map[tableName];

        public bool Contains(string tableName) => _map.ContainsKey(tableName);

        // =============================================================
        // 便捷映射方法
        // =============================================================

        /// <summary>
        /// 获取表名到槽位的映射：每个表包含哪些槽位
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlySet<TSlot>> GetTableToSlotsMap()
        {
            var tableToSlots = new Dictionary<string, HashSet<TSlot>>();
            foreach (var kvp in _map)
            {
                var tableName = kvp.Key;
                var slots = new HashSet<TSlot>(kvp.Value.Slots);
                tableToSlots[tableName] = slots;
            }
            return tableToSlots.ToDictionary(kvp => kvp.Key, kvp => kvp.Value as IReadOnlySet<TSlot>);
        }

        /// <summary>
        /// 获取槽位到表名的映射：每个槽位出现在哪些表中
        /// </summary>
        public IReadOnlyDictionary<TSlot, IReadOnlySet<string>> GetSlotToTablesMap()
        {
            return _slotToTables.ToDictionary(kvp => kvp.Key, kvp => kvp.Value as IReadOnlySet<string>);
        }

        // =============================================================
        // 更新操作
        // =============================================================

        /// <summary>
        /// 某槽位被确定（slot -> confirmedValue）
        /// removeThisCol = true 时：在候选表中移除这个槽位列
        /// </summary>
        public void UpdateSlotValueConfirmed(TSlot slot, TValue confirmedValue, bool removeThisCol = false)
        {
            if (!_slotToTables.TryGetValue(slot, out var affectedTableNames))
                return; // 没有任何表包含这个槽位

            foreach (var tableName in affectedTableNames.ToList())
            {
                if (!_map.TryGetValue(tableName, out var table))
                    continue;

                // 保留在指定槽位值为confirmedValue的行
                var filtered = table.Where(row =>
                {
                    if (!row.TryGetValue(slot, out var v)) return false;
                    return v.Equals(confirmedValue);
                });

                // 如果需要移除该槽位列，并且过滤后仍有行存在，则执行投影操作
                if (removeThisCol && filtered.Rows.Count > 0)
                {
                    var slotsToKeep = filtered.Rows[0].Slots
                        .Where(s => !s.Equals(slot))
                        .ToArray();

                    filtered = filtered.Project(slotsToKeep);
                }

                _map[tableName] = filtered;
            }

            // 如果移除了该槽位列，则从槽位到表的映射中移除该槽位
            if (removeThisCol)
            {
                _slotToTables.Remove(slot);
            }
        }

        /// <summary>
        /// 根据新的候选值域对所有表进行剪枝
        /// </summary>
        public void PruneByDomain(List<TValue> newDomain)
        {
            var counter = CounterUtils.CountValues(newDomain);

            foreach (var kvp in _map.ToList())
            {
                var tableName = kvp.Key;
                var table = kvp.Value;

                var filtered = table.Where(row =>
                {
                    var rowValues = row.Values.ToArray();
                    return CounterUtils.IsValidMultiset(counter, rowValues);
                });

                _map[tableName] = filtered;
            }
        }

        // =============================================================
        // 查询和访问方法
        // =============================================================

        public IEnumerable<CandidateTable<TSlot, TValue>> Tables => _map.Values;

        /// <summary>
        /// 获取指定槽位的候选值域，来自所有包含该槽位的表
        /// </summary>
        public HashSet<TValue> GetDomain(TSlot slot)
        {
            var domain = new HashSet<TValue>();
            if (_slotToTables.TryGetValue(slot, out var tableNames))
            {
                foreach (var tableName in tableNames)
                {
                    if (_map.TryGetValue(tableName, out var table) && table.Domain.TryGetValue(slot, out var slotDomain))
                    {
                        domain.UnionWith(slotDomain);
                    }
                }
            }
            return domain;
        }

        /// <summary>
        /// 检查是否存在包含指定槽位的表
        /// </summary>
        public bool ContainsSlot(TSlot slot)
        {
            return _slotToTables.ContainsKey(slot);
        }

        /// <summary>
        /// 获取包含指定槽位的所有表名
        /// </summary>
        public IEnumerable<string> GetTablesContainingSlot(TSlot slot)
        {
            return _slotToTables.TryGetValue(slot, out var tableNames) ? tableNames : Enumerable.Empty<string>();
        }

        // =============================================================
        // 其他操作
        // =============================================================

        public CandidateDomainManager<TSlot, TValue> Clone()
        {
            var manager = new CandidateDomainManager<TSlot, TValue>();
            foreach (var kvp in _map)
            {
                manager.Add(kvp.Key, kvp.Value.Clone());
            }
            return manager;
        }

        /// <summary>
        /// 清空所有表和映射关系
        /// </summary>
        public void Clear()
        {
            _map.Clear();
            _slotToTables.Clear();
        }

        /// <summary>
        /// 获取所有表名
        /// </summary>
        public IEnumerable<string> TableNames => _map.Keys;

        /// <summary>
        /// 获取表数量
        /// </summary>
        public int TableCount => _map.Count;
    }
}