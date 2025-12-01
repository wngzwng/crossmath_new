using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Types;

namespace CrossMath.Core.HoleDiggers;

/// <summary>
/// 挖空位置状态管理类，封装了挖空过程中所有位置的状态信息
/// </summary>
public sealed class HollowPositionState
{
    // ============================ 字段 ============================

    /// <summary>
    /// 已成功挖空的点（即题目中留空的格子）
    /// </summary>
    public HashSet<RowCol> SuccessfulHollowPositions { get; } = new();

    /// <summary>
    /// 挖空失败的点（死点），这些点在后续挖空过程中需要避免选择
    /// </summary>
    public HashSet<RowCol> FailedHollowPositions { get; } = new();

    /// <summary>
    /// 盘面所有的数字格子坐标（完整答案中包含数字的位置）
    /// </summary>
    public HashSet<RowCol> NumberCellPositions { get; } = new();

    /// <summary>
    /// 盘面所有的操作符格子坐标（+、-、×、÷ 等）不包含等号
    /// </summary>
    public HashSet<RowCol> OperatorCellPositions { get; } = new();

    // ============================ 只读属性 ============================

    /// <summary>
    /// 当前已成功挖空的个数
    /// </summary>
    public int SuccessfulCount => SuccessfulHollowPositions.Count;

    /// <summary>
    /// 当前死点个数
    /// </summary>
    public int FailedCount => FailedHollowPositions.Count;

    /// <summary>
    /// 数字格总数
    /// </summary>
    public int NumberCellCount => NumberCellPositions.Count;

    /// <summary>
    /// 操作符格总数
    /// </summary>
    public int OperatorCellCount => OperatorCellPositions.Count;

    /// <summary>
    /// 当前仍可挖空的数字格（未挖空且非死点的数字格）
    /// </summary>
    public IEnumerable<RowCol> AvailableNumberCells
        => NumberCellPositions.Except(SuccessfulHollowPositions).Except(FailedHollowPositions);

    /// <summary>
    /// 当前仍可挖空的操作符格（未挖空且非死点的操作符格）
    /// </summary>
    public IEnumerable<RowCol> AvailableOperatorCells
        => OperatorCellPositions.Except(SuccessfulHollowPositions).Except(FailedHollowPositions);

    /// <summary>
    /// 当前可挖空的数字格数量
    /// </summary>
    public int AvailableNumberCellCount => AvailableNumberCells.Count();

    /// <summary>
    /// 当前可挖空的操作符格数量
    /// </summary>
    public int AvailableOperatorCellCount => AvailableOperatorCells.Count();

    // ============================ 方法 ============================

    /// <summary>
    /// 根据算式布局初始化数字格和操作符格的位置集合
    /// </summary>
    public void InitializeFromLayouts(IEnumerable<ExpressionLayout> layouts)
    {
        NumberCellPositions.Clear();
        OperatorCellPositions.Clear();

        if (layouts == null) return;

        foreach (var layout in layouts)
        {
            foreach (var (cell, cellType) in layout.CellsWithTypes())
            {
                if (cellType == CellType.Number)
                {
                    NumberCellPositions.Add(cell);
                }

                if (cellType == CellType.Operator)
                {
                    OperatorCellPositions.Add(cell);
                }
            }
        }
    }

    /// <summary>
    /// 判断一个位置是否为有效的挖空候选点（必须是数字格或操作符格，且未被挖空、未标记为死点）
    /// </summary>
    public bool IsValidCandidate(RowCol position)
    {
        return (NumberCellPositions.Contains(position) || OperatorCellPositions.Contains(position))
               && !SuccessfulHollowPositions.Contains(position)
               && !FailedHollowPositions.Contains(position);
    }

    /// <summary>
    /// 标记一个位置为成功挖空
    /// </summary>
    public bool MarkAsSuccessful(RowCol position)
    {
        FailedHollowPositions.Remove(position);
        return SuccessfulHollowPositions.Add(position);
    }

    /// <summary>
    /// 标记一个位置为挖空失败（死点）
    /// </summary>
    public bool MarkAsFailed(RowCol position)
    {
        SuccessfulHollowPositions.Remove(position);
        return FailedHollowPositions.Add(position);
    }

    /// <summary>
    /// 清空挖空状态（保留数字格和操作符格的位置信息）
    /// </summary>
    public void ClearHollowState()
    {
        SuccessfulHollowPositions.Clear();
        FailedHollowPositions.Clear();
    }

    /// <summary>
    /// 完全清空所有数据（包括数字格和操作符格信息）
    /// </summary>
    public void ClearAll()
    {
        SuccessfulHollowPositions.Clear();
        FailedHollowPositions.Clear();
        NumberCellPositions.Clear();
        OperatorCellPositions.Clear();
    }
}