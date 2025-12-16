using CrossMath.Core.CandidateDomains;
using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.ExpressionSolvers;
using CrossMath.Core.ExpressionSolvers.ExpressionValidators;
using CrossMath.Core.ExpressionSolvers.NumberPools;
using CrossMath.Core.ExpressionSolvers.OperatorPools;
using CrossMath.Core.ExpressionSolvers.SolverProviders;
using CrossMath.Core.Types;
using CrossMath.Core.Utils;

namespace CrossMath.Core.Evaluation.LocalDifficulty;

public abstract class LocalDifficultyBase : ILocalDifficultyLayer
{
    public abstract int Difficulty { get; }

    public abstract void Evaluate(LocalDifficultyContext ctx);

    // =========================================================
    // 单表分析：只判断 + 记录，不修改盘面、不产生分支
    // =========================================================

    protected virtual bool AnalyzeCandidateTable(
        LocalDifficultyContext ctx,
        CandidateTable<RowCol, string> table)
    {
        // Local 只关心“是否唯一可解”
        if (!table.IsSingle)
            return false;

        var map = table[0].ToDictionary();

        // 校验：是否是合法多重集（分析层允许）
        var counts = CounterUtils.CountValues(ctx.Board.PossibleAnswers);
        if (!CounterUtils.IsValidMultiset(counts, map.Values.ToArray()))
            return false;

        // 只记录难度证据
        foreach (var pos in map.Keys)
        {
            ctx.Result.MarkDifficulty(pos, Difficulty);
        }

        return true;
    }

    // =========================================================
    // SolveContext：只读分析环境
    // =========================================================

    protected virtual ExpressionSolveContext CreateSolveContext(LocalDifficultyContext ctx)
    {
        return new ExpressionSolveContext
        {
            NumPool   = NumberPoolFactory.Create(ctx.Board.GetAnswerNumbers()),
            OpPool    = OperatorPoolFactory.Discrete(ctx.Board.GetAnswerOperators()),
            Validator = new ExpressionValidator(ValidationMode.FullDiscreteConsume)
        };
    }

    // =========================================================
    // 第一阶段：构建候选信息（纯收集）
    // =========================================================

    protected CandidateDomainManager<RowCol, string> BuildCandidateManager(
        LocalDifficultyContext ctx,
        IReadOnlyList<ExpressionLayout> layouts)
    {
        var manager = new CandidateDomainManager<RowCol, string>();
        var solveCtx = CreateSolveContext(ctx);

        foreach (var layout in layouts)
        {
            var origin = layout.ToExpression(ctx.Board);
            var solved = ctx.Solver.Solve(origin, solveCtx).ToList();
            if (solved.Count == 0)
                continue;

            var table = layout.BuildCandidateTable(origin, solved);
            if (table.Count == 0)
                continue;

            // 这里的 key 仅用于调试 / trace，不参与逻辑
            manager.Add(layout.ToString(), table);
        }

        return manager;
    }

    // =========================================================
    // 第二阶段：分析候选信息（纯判断）
    // =========================================================

    protected void AnalyzeCandidateManager(
        LocalDifficultyContext ctx,
        CandidateDomainManager<RowCol, string> manager)
    {
        foreach (var table in manager.Tables)
        {
            AnalyzeCandidateTable(ctx, table);
        }
    }
}
