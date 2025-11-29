using CrossMath.Core.CandidateDomains;
using CrossMath.Core.Types;

namespace CrossMath.Core.CSP;

public class CrossMathCSP
{
    private IReadOnlyDictionary<RowCol, IReadOnlySet<string>> _posToRelatedLayoutIds;
    private IReadOnlyDictionary<string, IReadOnlySet<RowCol>> _layoutIdToHoles;
    private Dictionary<RowCol, HashSet<string>> _holeDomains = new();

    public CSPResult RunPropagation(CandidateDomainManager<RowCol, string> manager, List<string> candidates)
    {
        Reset();
        Build(manager);
        InitHoleCandidate(candidates);

        var holes = _holeDomains.Keys.ToList();
        foreach (var pos in holes)
        {
            SpreadFromCell(pos, manager);
        }

        return new CSPResult(_holeDomains, manager);
    }

    public void Reset()
    {
        _holeDomains.Clear();
    }

    public void Build(CandidateDomainManager<RowCol, string> manager)
    {
        _posToRelatedLayoutIds = manager.GetSlotToTablesMap();
        _layoutIdToHoles = manager.GetTableToSlotsMap();
    }

    public void InitHoleCandidate(List<string> candidates)
    {
        foreach (var pos in _posToRelatedLayoutIds.Keys)
        {
            _holeDomains[pos] = new HashSet<string>(candidates);
        }
    }

    private void SpreadFromCell(RowCol cell, CandidateDomainManager<RowCol, string> manager)
    {
        var effectLayoutIds = new HashSet<string>();

        if (!_posToRelatedLayoutIds.TryGetValue(cell, out var relatedLayoutIds))
            return;

        foreach (var relatedId in relatedLayoutIds)
        {
            if (!_layoutIdToHoles.TryGetValue(relatedId, out var holes))
                continue;

            var table = manager.Get(relatedId);
            if (!table.Domain.TryGetValue(cell, out var cellDomain))
                continue;

            var holeDomain = _holeDomains[cell];
            var oldCount = holeDomain.Count;
            holeDomain.IntersectWith(cellDomain);
            var updatedCount = holeDomain.Count;

            if (updatedCount < oldCount)
            {
                effectLayoutIds.Add(relatedId);
            }
        }

        if (effectLayoutIds.Count > 0)
        {
            foreach (var layoutId in effectLayoutIds)
            {
                SpreadFromLayout(layoutId, manager);
            }
        }
    }

    private void SpreadFromLayout(string layoutId, CandidateDomainManager<RowCol, string> manager)
    {
        var table = manager.Get(layoutId);
        var effectHoles = new HashSet<RowCol>();

        // 对该layout中所有槽位的域进行交集操作
        foreach (var hole in table.Slots)
        {
            if (!table.Domain.TryGetValue(hole, out var holeDomainFromTable))
                continue;

            var currentHoleDomain = _holeDomains[hole];
            var oldCount = currentHoleDomain.Count;
            currentHoleDomain.IntersectWith(holeDomainFromTable);
            var updatedCount = currentHoleDomain.Count;

            if (updatedCount < oldCount)
            {
                effectHoles.Add(hole);
            }
        }

        // 如果有域发生变化，则对该表的候选行进行过滤
        if (effectHoles.Count > 0)
        {
            var filteredTable = table.Where(candidateRow =>
            {
                foreach (var slot in candidateRow.Slots)
                {
                    if (!candidateRow.TryGetValue(slot, out var value))
                        return false;

                    if (!_holeDomains[slot].Contains(value))
                        return false;
                }
                return true;
            });

            // 更新管理器中的表
            manager.Add(layoutId, filteredTable);

            // 对受影响的空位继续传播
            foreach (var effectHole in effectHoles)
            {
                SpreadFromCell(effectHole, manager);
            }
        }
    }
}