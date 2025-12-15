using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Types;
using CrossMath.Core.Utils;

namespace CrossMath.Core.HoleDiggers.HollowOutStrategies;


/// <summary>
/// 随机策略：所有候选位置等概率选择
/// </summary>
public class RandomStrategy:IHollowOutStrategy
{
    public bool AllowHollowOutOperator => _allowHollowOutOperator;
    private bool _allowHollowOutOperator = false;

    private Dictionary<RowCol, int>? _allAllowRowColWeights = null;

    public RandomStrategy(bool allowHollowOutOperator = false)
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
        var rowColWeights = rowColCellTypeMap
            .Where(pair => AllowCellType(pair.Value))
            .ToDictionary(kv => kv.Key, kv => 1);
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
    
}