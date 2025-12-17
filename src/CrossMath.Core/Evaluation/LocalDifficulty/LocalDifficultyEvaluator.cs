using CrossMath.Core.ExpressionSolvers.SolverProviders;
using CrossMath.Core.Models;
using CrossMath.Core.Types;
using Microsoft.Extensions.Logging;

namespace CrossMath.Core.Evaluation.LocalDifficulty;

/// <summary>
/// 局部难度评估器（Local Difficulty Evaluator）
///
/// 职责：
/// - 负责按难度顺序调度各个 LocalDifficultyLayer
/// - 协调局部解题推理流程（candidate / 约束传播 / 推断）
/// - 统一产出局部难度评估结果及中间推理产物
///
/// 该类是一个“调度器 / Pipeline Orchestrator”，
/// 本身不实现具体难度逻辑，所有策略均由 Layer 决定。
/// </summary>
public sealed class LocalDifficultyEvaluator
{
    /// <summary>
    /// 按 Difficulty 升序排列的局部难度层
    /// </summary>
    private readonly IReadOnlyList<ILocalDifficultyLayer> _layers;

    /// <summary>
    /// 日志（用于阶段、性能或调试输出）
    /// </summary>
    private readonly ILogger<LocalDifficultyEvaluator> _logger;

    public LocalDifficultyEvaluator(
        IEnumerable<ILocalDifficultyLayer> layers,
        ILoggerFactory loggerFactory)
    {
        // 统一保证 Layer 的执行顺序由 Difficulty 决定
        _layers = layers
            .OrderBy(l => l.Difficulty)
            .ToList();

        _logger = loggerFactory.CreateLogger<LocalDifficultyEvaluator>();
    }

    /// <summary>
    /// 创建默认的局部难度评估器（内置所有标准 Layer）
    /// </summary>
    public static LocalDifficultyEvaluator CreateDefault(ILoggerFactory loggerFactory)
    {
        return new LocalDifficultyEvaluator(
            new ILocalDifficultyLayer[]
            {
                new LocalDifficultyLayerOne(),
                new LocalDifficultyLayerTwo(),
                new LocalDifficultyThree(),
                new LocalDifficultyFour()
            },
            loggerFactory);
    }

    /// <summary>
    /// 为指定棋盘创建一个全新的评估上下文
    ///
    /// Context 持有：
    /// - 棋盘数据
    /// - 表达式布局缓存
    /// - Solver
    /// - 局部评估结果与中间推理数据
    /// </summary>
    public static LocalDifficultyContext CreateContext(BoardData board)
    {
        return new LocalDifficultyContext(
            board: board,
            solver: ExpressionSolverProvider.CreateDefault());
    }

    /// <summary>
    /// 执行完整的局部难度评估流程
    ///
    /// 会：
    /// - 重置 Context
    /// - 按顺序执行所有可评估的 Layer
    /// - 汇总并返回所有评估结果与中间产物
    ///
    /// 这是唯一的“核心执行入口”，
    /// 其他快捷方法均应复用此方法。
    /// </summary>
    public LocalDifficultyEvaluation Evaluate(LocalDifficultyContext context)
    {
        // 清空上一次评估产生的所有状态
        context.Reset();

        // 依次执行各个难度层
        foreach (var layer in _layers)
        {
            if (!layer.CanEvaluate(context))
                continue;

            layer.Evaluate(context);
        }

        // 汇总 Context 中的结果，生成只读评估产物
        return new LocalDifficultyEvaluation
        {
            // 每个格子的最小局部难度
            MinDifficultyPerCell = context.Result.MinDifficultyPerCell
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),

            // 每个格子的候选域（通常由 CSP / 约束传播产生）
            CandidateMapAtCell = context.CandidateMapAtCell
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
        };
    }

    /// <summary>
    /// 快捷方法：只关心每个格子的最小局部难度
    ///
    /// 适用于：
    /// - 统计分析
    /// - 全局难度聚合
    /// - 旧逻辑兼容
    /// </summary>
    public Dictionary<RowCol, int> EvaluateMinDifficulty(LocalDifficultyContext context)
    {
        return Evaluate(context).MinDifficultyPerCell
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// 快捷方法：直接从 BoardData 计算最小局部难度
    ///
    /// 内部会自动创建 Context，不污染外部状态
    /// </summary>
    public Dictionary<RowCol, int> EvaluateMinDifficulty(BoardData board)
    {
        return EvaluateMinDifficulty(CreateContext(board));
    }
}
