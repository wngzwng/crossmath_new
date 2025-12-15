using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.HoleDiggers.HoleCount;
using CrossMath.Core.HoleDiggers.HoleVadidators;
using CrossMath.Core.HoleDiggers.HollowOutStrategies;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.HoleDiggers;

/// <summary>
/// 挖空上下文类，包含挖空过程中所需的所有相关数据和策略
/// 支持两种期望挖空数设定模式：固定值 或 动态计算（基于 HoleCountType）
/// </summary>
public sealed class HollowOutContext
{
    /// <summary>
    /// 原始完整答案棋盘（永远不变）
    /// </summary>
    public BoardData OriginalBoard { get; }

    /// <summary>
    /// 当前正在工作的棋盘
    /// </summary>
    public BoardData WorkingBoard { get; private set; }

    /// <summary>
    /// 算式布局信息
    /// </summary>
    public List<ExpressionLayout> Layouts { get; } = new();

    /// <summary>
    /// 当前最终期望的挖空数量（可能在运行中动态更新）
    /// </summary>
    public int ExpectedHollowCount { get; private set; }

    /// <summary>
    /// 【可选】动态计算状态（当使用 HoleCountType 策略时需要）
    /// </summary>
    public ExpectedHollowCountState? DynamicState { get; private set; }

    /// <summary>
    /// 是否使用动态计算模式（true = 使用 DynamicState 动态更新 ExpectedHollowCount）
    /// </summary>
    public bool IsDynamicMode => DynamicState != null;

    /// <summary>
    /// 挖空策略
    /// </summary>
    public IHollowOutStrategy HollowOutStrategy { get; set; }

    /// <summary>
    /// 挖空验证器
    /// </summary>
    public IHoleValidator HoleValidator { get; set; }

    /// <summary>
    /// 位置状态管理
    /// </summary>
    public HollowPositionState PositionState { get; } = new();

    // ======================== 只读统计属性 ========================
    public int CurrentHollowCount => PositionState.SuccessfulCount;
    public bool HasReachedExpectedCount => CurrentHollowCount >= ExpectedHollowCount;
    public int RemainingHollowCount => Math.Max(0, ExpectedHollowCount - CurrentHollowCount);

    // ======================== 构造函数（支持两种模式） ========================

    private HollowOutContext(BoardData originalBoard)
    {
        OriginalBoard = originalBoard ?? throw new ArgumentNullException(nameof(originalBoard));
        WorkingBoard = originalBoard.Clone();
    }

    /// <summary>
    /// 【固定值模式】直接指定期望挖空数
    /// </summary>
    public static HollowOutContext CreateFixed(
        BoardData originalBoard,
        int expectedHollowCount,
        IHollowOutStrategy hollowOutStrategy,
        IHoleValidator holeValidator,
        IEnumerable<ExpressionLayout>? layouts = null)
    {
        var ctx = new HollowOutContext(originalBoard)
        {
            ExpectedHollowCount = expectedHollowCount,
            HollowOutStrategy = hollowOutStrategy ?? throw new ArgumentNullException(nameof(hollowOutStrategy)),
            HoleValidator = holeValidator ?? throw new ArgumentNullException(nameof(holeValidator))
        };

        ctx.InitializeLayouts(layouts);
        return ctx;
    }

    /// <summary>
    /// 【动态计算模式】使用 HoleCountType 策略，ExpectedHollowCount 会在第一阶段后自动计算
    /// </summary>
    public static HollowOutContext Create(
        BoardData originalBoard,
        HoleCountType holeCountType,
        IHollowOutStrategy hollowOutStrategy,
        IHoleValidator holeValidator,
        IEnumerable<ExpressionLayout>? layouts = null)
    {
       var dynamicState = ExpectedHollowCountState.Create(holeCountType);
        var ctx = new HollowOutContext(originalBoard)
        {
            DynamicState = dynamicState,
            // 初始给一个极大值，防止第一阶段提前停止
            ExpectedHollowCount = int.MaxValue,
            HollowOutStrategy = hollowOutStrategy ?? throw new ArgumentNullException(nameof(hollowOutStrategy)),
            HoleValidator = holeValidator ?? throw new ArgumentNullException(nameof(holeValidator))
        };

        ctx.InitializeLayouts(layouts);
        return ctx;
    }

    private void InitializeLayouts(IEnumerable<ExpressionLayout>? layouts)
    {
        if (layouts != null)
            Layouts.AddRange(layouts);
        else
            Layouts.AddRange(ExpressionLayoutBuilder.ExtractLayouts(OriginalBoard.Layout, [5, 7]));

        PositionState.InitializeFromLayouts(Layouts);
    }

    // ======================== 核心方法 ========================

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

    public RowCol? GetNextHoleCoordinate(IEnumerable<RowCol>? candidatePositions = null)
        => HollowOutStrategy?.GetNextHoleCoordinate(this, candidatePositions);

    public bool HasValidCandidates()
        => PositionState.AvailableNumberCells.Any() || PositionState.AvailableOperatorCells.Any();

    public void Reset()
    {
        WorkingBoard = OriginalBoard.Clone();
        PositionState.ClearHollowState();
        // Layouts 不清空，保持一致
    }

    public BoardData GetResultBoard() => WorkingBoard.Clone();

    // ======================== 关键新增：动态更新期望值 ========================

    /// <summary>
    /// 在第一阶段结束后调用：根据 DynamicState 和实际 minHoleCount 计算最终期望挖空数
    /// </summary>
    /// <param name="phaseOneMinHollowCount">第一阶段得到的约束下最大挖空数</param>
    /// <returns>最终计算出的期望挖空数</returns>
    public int UpdateExpectedHollowCountAfterPhaseOne(int phaseOneMinHollowCount)
    {
        if (!IsDynamicMode)
            throw new InvalidOperationException("只有动态模式才能调用 UpdateExpectedHollowCountAfterPhaseOne");

        int formulaCount = Layouts.Count;
        // 动态计算最终期望值
        ExpectedHollowCount = DynamicState.CalculateExpected(phaseOneMinHollowCount, formulaCount);

        return ExpectedHollowCount;
    }
}