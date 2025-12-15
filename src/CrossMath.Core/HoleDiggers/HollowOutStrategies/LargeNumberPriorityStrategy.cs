using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Types;
using CrossMath.Core.Utils;

namespace CrossMath.Core.HoleDiggers.HollowOutStrategies;


/// <summary>
/// 大数字优先策略：优先挖空数字较大的数字<br/>
/// 每个数字的权重为它的数值
/// </summary>
public class LargeNumberPriorityStrategy: IHollowOutStrategy
{
    public bool AllowHollowOutOperator => false;
    
    private Dictionary<RowCol, int>? _allAllowRowColWeights = null;

    public RowCol? GetNextHoleCoordinate(HollowOutContext context, IEnumerable<RowCol>? candidatePositions = null)
    {
        var weights = GetWeights(context, candidatePositions);
        if (weights == null)
        {
            return null;
        }

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
            rowcol => _allAllowRowColWeights[rowcol]
        );
    }

    public HashSet<RowCol> GetCandidatePosition(HollowOutContext context,
        IEnumerable<RowCol>? candidatePositions = null)
    {
        var availableCells = context.PositionState.AvailableNumberCells;

        var targetPositions = candidatePositions != null
            ? candidatePositions.Intersect(availableCells)
            : availableCells;

        return targetPositions.ToHashSet();
    }


    public Dictionary<RowCol, int> GetAllAllowRowColWeights(HollowOutContext context)
    {
        var board = context.OriginalBoard;
        var allNumbersPos = GetRowColCellTypeMap(context.Layouts)
            .Where(kv => kv.Value == CellType.Number)
            .Select(kv => kv.Key)
            .ToHashSet();

        return allNumbersPos.ToDictionary(
            rowcol => rowcol,
            rowcol => board.TryGetValue(rowcol, out var value)
                ? int.TryParse(value, out int number) ? number : 0
                : 0
        );
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