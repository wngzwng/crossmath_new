using CrossMath.Core.Types;
using CrossMath.Core.Utils;

namespace CrossMath.Core.HoleDiggers.HollowOutStrategies;
/// <summary>
/// 重复数字优先挖空策略：优先挖空在完整棋盘中出现次数较多的数字对应的格子。<br/>
/// 每个候选位置的权重等于其数字在原始完整棋盘（OriginalBoard）中的总出现次数。<br/>
/// 通过缓存全局数字频次，避免重复统计提升性能。
/// </summary>
public class FrequentNumberPriorityStrategy : IHollowOutStrategy
{
    public bool AllowHollowOutOperator => false;
    
    private Dictionary<int, int>? _globalFrequencyCache;

    /// <summary>
    /// 获取下一个建议挖空的坐标。
    /// </summary>
    /// <param name="context">当前的挖空上下文</param>
    /// <param name="candidatePositions">可选的候选位置集合，若为 null 则使用所有可挖空位置</param>
    /// <returns>根据权重随机选中的坐标，若无有效候选则返回 null</returns>
    public RowCol? GetNextHoleCoordinate(HollowOutContext context, IEnumerable<RowCol>? candidatePositions = null)
    {
        var weights = GetWeights(context, candidatePositions);
        return weights != null ? WeightedRandom.From(weights).Next() : null;
    }

    /// <summary>
    /// 计算候选位置的权重字典。
    /// 权重定义为：该位置数字在原始完整棋盘中的出现频次。
    /// </summary>
    /// <param name="context">当前的挖空上下文</param>
    /// <param name="candidatePositions">可选的候选位置集合，若为 null 则使用所有可挖空位置</param>
    /// <returns>位置到权重的映射，若无有效候选位置则返回 null</returns>
    public Dictionary<RowCol, int>? GetWeights(HollowOutContext context, IEnumerable<RowCol>? candidatePositions = null)
    {
        // 缓存全局数字频次（基于 OriginalBoard，不随挖空变化）
        _globalFrequencyCache ??= CalculateGlobalFrequency(context);

        var positionToNumber = GetCandidatePositionNumbers(context, candidatePositions);
        if (positionToNumber == null || positionToNumber.Count == 0)
        {
            return null;
        }

        // 为每个候选位置分配权重：其数字的全局出现次数
        return positionToNumber.ToDictionary(
            kvp => kvp.Key,
            kvp => _globalFrequencyCache.TryGetValue(kvp.Value, out var freq) ? freq : 0);
    }

    /// <summary>
    /// 计算原始棋盘中所有数字的出现频次（仅计算一次并缓存）。
    /// </summary>
    private Dictionary<int, int> CalculateGlobalFrequency(HollowOutContext context)
    {
        var allNumbers = context.OriginalBoard.GetAllNumbers();
        return CounterUtils.CountValues(allNumbers);
    }

    /// <summary>
    /// 获取候选位置的有效数字映射（位置 → 数字）。
    /// </summary>
    private Dictionary<RowCol, int>? GetCandidatePositionNumbers(
        HollowOutContext context,
        IEnumerable<RowCol>? candidatePositions)
    {
        var board = context.WorkingBoard;
        var availableCells = context.PositionState.AvailableNumberCells;

        var targetPositions = candidatePositions != null
            ? candidatePositions.Intersect(availableCells)
            : availableCells;

        var result = new Dictionary<RowCol, int>();

        foreach (var pos in targetPositions)
        {
            if (board.TryGetValue(pos, out var cellValue) &&
                int.TryParse(cellValue, out int number))
            {
                result[pos] = number;
            }
        }

        return result.Count > 0 ? result : null;
    }
}