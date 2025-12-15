using CrossMath.Core.Types;

namespace CrossMath.Core.Expressions.Layout;

public static class ExpressionLayoutGraphUtils
{
    public static Dictionary<RowCol, HashSet<string>> BuildPosToExprMap(IEnumerable<ExpressionLayout> exprLayouts)
    {
        var map = new Dictionary<RowCol, HashSet<string>>();

        foreach (var exprlayout in exprLayouts)
        {
            foreach (var pos in exprlayout.Cells)
            {
                if (!map.TryGetValue(pos, out var set))
                {
                    set = new HashSet<string>();
                    map[pos] = set;
                }
                set.Add(exprlayout.Id.Value);
            }
        }
        return map;
    }

    public static Dictionary<string, HashSet<RowCol>> BuildExprToPosMap(IEnumerable<ExpressionLayout> exprlayouts)
    {
        return exprlayouts.ToDictionary(
            e => e.Id.Value,
            e => new HashSet<RowCol>(e.Cells));
    }

    public static Dictionary<string, HashSet<string>> BuildIntersectionGraph(IEnumerable<ExpressionLayout> exprlayouts)
    {
        var posToExpr = BuildPosToExprMap(exprlayouts);
        var graph = new Dictionary<string, HashSet<string>>();

        foreach (var kv in posToExpr)
        {
            var ids = kv.Value;
            if (ids.Count < 2) continue;

            foreach (var idA in ids)
            {
                if (!graph.TryGetValue(idA, out var set))
                {
                    set = new HashSet<string>();
                    graph[idA] = set;
                }

                foreach (var idB in ids)
                {
                    if (idA != idB) set.Add(idB);
                }
            }
        }
        return graph;
    }
    
    public static Dictionary<RowCol, CellType> BuildPosToCellTypeMap(IEnumerable<ExpressionLayout> exprLayouts)
    {
        var map = new Dictionary<RowCol, CellType>();

        foreach (var exprlayout in exprLayouts)
        {
            foreach (var (pos, cellType) in exprlayout.CellsWithTypes())
            {
                map[pos] = cellType;
            }
        }
        return map;
    }
}
