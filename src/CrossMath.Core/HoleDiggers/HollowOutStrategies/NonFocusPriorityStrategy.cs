using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Types;
using CrossMath.Core.Utils;

namespace CrossMath.Core.HoleDiggers.HollowOutStrategies;

/// <summary>
/// 非焦点优先策略：优先选择非交点位置<br/>
/// 每个交点格权重3，每个非交点格权重7
/// </summary>
public class NonFocusPriorityStrategy : IHollowOutStrategy
{
    public bool AllowHollowOutOperator => _allowHollowOutOperator;
    private bool _allowHollowOutOperator = false;
    
    private const int IntersectionWeight = 3;
    private const int NonFocusWeight = 7;

    private Dictionary<RowCol, int>? _allAllowRowColWeights = null;

    public NonFocusPriorityStrategy(bool allowHollowOutOperator = false)
    {
        _allowHollowOutOperator = allowHollowOutOperator;
    }
    
    public RowCol? GetNextHoleCoordinate(HollowOutContext context, IEnumerable<RowCol>? candidatePositions = null)
    {
        var weights= GetWeights(context, candidatePositions);
        if (weights == null) return null;

        return WeightedRandom.From(weights).Next();
    }

    public Dictionary<RowCol, int>? GetWeights(HollowOutContext context, IEnumerable<RowCol>? candidatePositions = null)
    {
        _allAllowRowColWeights ??= GetAllAllowRowColWeights(context);
        var targetPositions = GetCandidatePosition(context, candidatePositions)
            .Where(rowcol => _allAllowRowColWeights.ContainsKey(rowcol))
            .ToHashSet();

        if (targetPositions.Count == 0)
        {
            return null;
        }
        
        return targetPositions.ToDictionary(
            rowcol => rowcol, 
            rowcol => _allAllowRowColWeights[rowcol]);
    }

    /// <summary>
    /// 获取非空格和非死点的位置
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidatePositions"></param>
    /// <returns></returns>
    public HashSet<RowCol> GetCandidatePosition(HollowOutContext context,
        IEnumerable<RowCol>? candidatePositions = null)
    {
        var availableCells = context.PositionState.AvailableNumberCells.ToHashSet();
        if (AllowHollowOutOperator)
        {
            availableCells.UnionWith(context.PositionState.AvailableOperatorCells.ToHashSet());
        }
        var targetPositions = candidatePositions != null
            ? candidatePositions.Intersect(availableCells)
            : availableCells;

        return targetPositions.ToHashSet();
    }

    public Dictionary<RowCol, int> GetAllAllowRowColWeights(HollowOutContext context)
    {
        var rowColCellTypeMap = GetRowColCellTypeMap(context.Layouts);
        var intersectionSet = GetRowColIntersectionSet(context.Layouts);
        
        var rowColWeights = rowColCellTypeMap
            .Where(pair => AllowCellType(pair.Value))
            .ToDictionary(kv => kv.Key, kv => intersectionSet.Contains(kv.Key) ? IntersectionWeight : NonFocusWeight);
        return rowColWeights;
    }

    public bool AllowCellType(CellType cellType)
    {
        if (AllowHollowOutOperator)
        {
            return cellType == CellType.Number || cellType == CellType.Operator;
        }
        return cellType == CellType.Number;
    }

    private Dictionary<RowCol, CellType> GetRowColCellTypeMap(IEnumerable<ExpressionLayout> layouts)
    {
        var rowColToCellMap = new Dictionary<RowCol, CellType>();
        foreach (var expLayout in layouts)
        {
            foreach (var (pos, cellType) in expLayout.CellsWithTypes())
            {
                rowColToCellMap.TryAdd(pos, cellType);
            }
        }
        return rowColToCellMap;
    }

    private HashSet<RowCol> GetRowColIntersectionSet(IEnumerable<ExpressionLayout> layouts)
    {
        var intersectionSet =  new HashSet<RowCol>();
        var rowColToLayoutIds = ExpressionLayoutGraphUtils.BuildPosToExprMap(layouts);
        foreach (var rowColToLayoutId in rowColToLayoutIds)
        {
            if (rowColToLayoutId.Value.Count > 1)
            {
                intersectionSet.Add(rowColToLayoutId.Key);
            }
        }
        return intersectionSet;
    }
}