using System;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.HoleDiggers;

/// <summary>
/// 负责整个挖空过程的协调与控制（纯粹的“流程控制器”）
/// </summary>
public sealed class HoleDigger
{
    /// <summary>
    /// 最大连续失败次数，防止极端情况下陷入死循环
    /// </summary>
    private const int MaxConsecutiveFailures = 500;

    /// <summary>
    /// 尝试挖空直到达到期望数量或无法继续
    /// </summary>
    /// <param name="ctx">挖空上下文</param>
    /// <param name="resultBoard">成功时返回挖空后的题目棋盘（克隆副本），失败返回 null</param>
    /// <returns>是否成功达到期望挖空数量</returns>
    public bool TryHollowOut(HollowOutContext ctx, out BoardData? resultBoard)
    {
        if (ctx == null) throw new ArgumentNullException(nameof(ctx));
        if (ctx.HollowOutStrategy == null) throw new InvalidOperationException("HollowOutStrategy 未设置");
        if (ctx.HoleValidator == null) throw new InvalidOperationException("HoleValidator 未设置");

        resultBoard = null;

        // 1. 重置上下文（恢复工作棋盘、清除成功/失败标记）
        ctx.Reset();

        int consecutiveFailures = 0;

        // 2. 主循环：持续挖空直到满足条件
        while (ctx.CurrentHollowCount < ctx.ExpectedHollowCount && ctx.HasValidCandidates())
        {
            RowCol? candidate = ctx.GetNextHoleCoordinate();

            // 策略已经没有建议位置 → 直接退出（防止无限循环）
            if (!candidate.HasValue)
                break;

            bool success = ctx.TryHollowOut(candidate.Value);

            if (success)
            {
                consecutiveFailures = 0; // 成功一次就重置计数
            }
            else
            {
                consecutiveFailures++;
                // 连续失败太多，基本可以判定已经陷入局部死胡同，直接放弃
                if (consecutiveFailures >= MaxConsecutiveFailures)
                    break;
            }
        }

        // 3. 判断最终结果
        bool successFinal = ctx.CurrentHollowCount >= ctx.ExpectedHollowCount;

        if (successFinal)
        {
            // 返回挖空后的题目棋盘（克隆，防止外部修改影响上下文）
            resultBoard = ctx.WorkingBoard.Clone();
        }

        return successFinal;
    }

    /// <summary>
    /// 临时修改期望挖空数量进行一次挖空（不影响 ctx.ExpectedHollowCount）
    /// </summary>
    public bool TryHollowOut(HollowOutContext ctx, int temporaryTargetCount, out BoardData? resultBoard)
    {
        if (temporaryTargetCount < 0)
            throw new ArgumentOutOfRangeException(nameof(temporaryTargetCount));

        int originalTarget = ctx.ExpectedHollowCount;
        ctx.ExpectedHollowCount = temporaryTargetCount;

        try
        {
            return TryHollowOut(ctx, out resultBoard);
        }
        finally
        {
            ctx.ExpectedHollowCount = originalTarget; // 恢复原始值
        }
    }

    /// <summary>
    /// 只挖空一个格子（常用于调试或逐步演示）
    /// </summary>
    public bool TrySingleHollowOut(HollowOutContext ctx)
    {
        if (ctx == null) throw new ArgumentNullException(nameof(ctx));
        if (!ctx.HasValidCandidates()) return false;

        RowCol? pos = ctx.GetNextHoleCoordinate();
        return pos.HasValue && ctx.TryHollowOut(pos.Value);
    }

    /// <summary>
    /// 检查当前上下文是否还能继续挖空（不依赖策略的顺序）
    /// </summary>
    public static bool CanContinue(HollowOutContext ctx)
    {
        return ctx != null &&
               ctx.CurrentHollowCount < ctx.ExpectedHollowCount &&
               ctx.HasValidCandidates();
    }
}