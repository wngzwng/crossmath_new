using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.HoleDiggers.HoleVadidators;
using CrossMath.Core.HoleDiggers.HollowOutStrategies;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.HoleDiggers;

/// <summary>
/// 挖空上下文类，包含挖空过程中所需的所有相关数据和策略
/// </summary>
public sealed class HollowOutContext
{
    /// <summary>
    /// 原始完整答案棋盘（永远不变）
    /// </summary>
    public BoardData OriginalBoard { get; }

    /// <summary>
    /// 当前正在工作的棋盘（挖空后会变成题目棋盘）
    /// </summary>
    public BoardData WorkingBoard { get; private set; }

    /// <summary>
    /// 算式布局信息（用于初始化数字格/操作符格）
    /// </summary>
    public List<ExpressionLayout> Layouts { get; } = new();

    /// <summary>
    /// 期望的挖空数量
    /// </summary>
    public int ExpectedHollowCount { get; set; }

    /// <summary>
    /// 挖空策略（决定下一个挖哪个格子）
    /// </summary>
    public IHollowOutStrategy HollowOutStrategy { get; set; }

    /// <summary>
    /// 挖空验证器（判断挖空后是否仍然唯一解）
    /// </summary>
    public IHoleValidator HoleValidator { get; set; }

    /// <summary>
    /// 所有位置状态的统一管理对象
    /// </summary>
    public HollowPositionState PositionState { get; } = new();

    // ======================== 只读统计属性 ========================

    public int CurrentHollowCount => PositionState.SuccessfulCount;

    public bool HasReachedExpectedCount => CurrentHollowCount >= ExpectedHollowCount;

    public int RemainingHollowCount => Math.Max(0, ExpectedHollowCount - CurrentHollowCount);

    // ======================== 构造函数 ========================

    public HollowOutContext(
        BoardData originalBoard,
        int expectedHollowCount,
        IHollowOutStrategy hollowOutStrategy,
        IHoleValidator holeValidator,
        IEnumerable<ExpressionLayout>? layouts = null)
    {
        OriginalBoard   = originalBoard ?? throw new ArgumentNullException(nameof(originalBoard));
        WorkingBoard    = originalBoard.Clone();               // 工作副本
        ExpectedHollowCount = expectedHollowCount;
        HollowOutStrategy    = hollowOutStrategy ?? throw new ArgumentNullException(nameof(hollowOutStrategy));
        HoleValidator        = holeValidator     ?? throw new ArgumentNullException(nameof(holeValidator));

        if (layouts != null)
            Layouts.AddRange(layouts);
        else
        {
            Layouts.AddRange(ExpressionLayoutBuilder.ExtractLayouts(OriginalBoard.Layout, [5, 7]));
        }

        // 根据布局初始化数字格和操作符格集合
        PositionState.InitializeFromLayouts(Layouts);
    }

    // ======================== 核心操作 ========================

    /// <summary>
    /// 尝试在指定位置挖空
    /// </summary>
    public bool TryHollowOut(RowCol position)
    {
        // 1. 必须是合法候选点
        if (!PositionState.IsValidCandidate(position))
            return false;

        // 2. 验证器检查（唯一解等）
        if (HoleValidator != null && !HoleValidator.IsValidHollowOut(WorkingBoard, position))
        {
            PositionState.MarkAsFailed(position);
            return false;
        }

        // 3. 真正执行挖空
        PositionState.MarkAsSuccessful(position);
        WorkingBoard.ExtractAnswer(position, out _);
        return true;
    }

    /// <summary>
    /// 通过策略获取下一个建议挖空的坐标
    /// </summary>
    public RowCol? GetNextHoleCoordinate()
    {
        return HollowOutStrategy?.GetNextHoleCoordinate(this);
    }

    /// <summary>
    /// 判断是否还有可挖空的候选点（不依赖策略的顺序）
    /// </summary>
    public bool HasValidCandidates()
    {
        // 只要还有未挖空且非死点的数字格或操作符格，就认为还有候选
        return PositionState.AvailableNumberCells.Any() ||
               PositionState.AvailableOperatorCells.Any();
    }

    /// <summary>
    /// 重置挖空状态（重新开始挖空时调用）
    /// </summary>
    public void Reset()
    {
        WorkingBoard = OriginalBoard.Clone();
        PositionState.ClearHollowState();   // 只清空成功/失败状态，保留数字格和操作符格信息
        Layouts.Clear();
        Layouts.AddRange(ExpressionLayoutBuilder.ExtractLayouts(OriginalBoard.Layout, [5, 7])); // 如果原始棋盘能提供布局的话
    }

    /// <summary>
    /// 获取当前挖空后的题目棋盘（外部调用完成后取结果）
    /// </summary>
    public BoardData GetResultBoard() => WorkingBoard.Clone();
}