using System.Reflection;
using CrossMath.Core.CandidateDomains;
using CrossMath.Core.Expressions.Core;
using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.ExpressionSolvers;
using CrossMath.Core.ExpressionSolvers.ExpressionValidators;
using CrossMath.Core.ExpressionSolvers.NumberPools;
using CrossMath.Core.ExpressionSolvers.OperatorPools;
using CrossMath.Core.ExpressionSolvers.SolverProviders;
using CrossMath.Core.Models;
using CrossMath.Core.Types;
using CrossMath.Core.Utils;

namespace CrossMath.Core.Evaluation;

public abstract class GlobalDifficultyLayerBase : IGlobalDifficultyLayer
{
    public abstract int Difficulty { get; }

    protected virtual bool UsePerLayoutProcessing => true;

    public bool TryEvaluate(GlobalDifficultyContext ctx,
        out IEnumerable<GlobalDifficultyContext>? branches)
    {
        branches = null;

        var layouts = ExtractTargetLayouts(ctx.Board, ctx);
        if (layouts.Count == 0)
            return false;

        if (!UsePerLayoutProcessing)
        {
            // 批处理模式检查
            EnsureOverride(nameof(HandleLayoutsBatch));
            return HandleLayoutsBatch(ctx, layouts, out branches);
        }

        // 单 layout 处理模式检查
        // EnsureOverride(nameof(HandleCandidateTable));
        return HandleLayoutsIndividually(ctx, layouts, out branches);
    }

    
    protected void EnsureOverride(string methodName)
    {
        var type = GetType();
        var method = type.GetMethod(methodName,
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        if (method == null)
            throw new InvalidOperationException($"{methodName} 方法不存在（非预期）。");

        bool overridden = method.DeclaringType != typeof(GlobalDifficultyLayerBase);

        if (!overridden)
        {
            throw new InvalidOperationException(
                $"Layer \"{type.Name}\" 需要 override {methodName}，否则行为未定义。");
        }
    }


    // =============================== 必须实现 ===============================

    protected abstract List<ExpressionLayout> ExtractTargetLayouts(
        BoardData board,
        GlobalDifficultyContext ctx);

    // =============================== 可选（per-layout 层才用） ===============================

    protected virtual bool HandleCandidateTable(
        GlobalDifficultyContext ctx,
        ExpressionLayout layout,
        CandidateTable<RowCol, string> table,
        out IEnumerable<GlobalDifficultyContext>? branches)
    {
        branches = null;
        var board = ctx.Board;
        if (table.IsSingle)
        {
            var map = table[0].ToDictionary();
            var counts = CounterUtils.CountValues(board.PossibleAnswers);
            if (!CounterUtils.IsValidMultiset(counts, map.Values.ToArray())) return false;
            
            ctx.UpdateDifficulty(Difficulty);
            
            foreach (var pos in map.Keys)
            {
                // 避免候选数乱序问题
                board.SetValueOnly(pos, map[pos]);
                board.RemovePossibleAnswer(map[pos]);

                ctx.DifficultyRecord[pos] = ctx.CurrentDifficulty;
            }

            return true;
        }

        return false;
    }

    // =============================== 可选（batch 层才用） ===============================

    protected virtual bool HandleLayoutsBatch(
        GlobalDifficultyContext ctx,
        List<ExpressionLayout> layouts,
        out IEnumerable<GlobalDifficultyContext>? branches)
    {
        branches = null;
        return false;
    }

    // =============================== 可选（SolveContext） ===============================

    protected virtual ExpressionSolveContext CreateSolveContext(GlobalDifficultyContext ctx)
    {
        return new ExpressionSolveContext
        {
            NumPool = NumberPoolFactory.Create(ctx.Board.GetAnswerNumbers()),
            OpPool  = OperatorPoolFactory.Discrete(ctx.Board.GetAnswerOperators()),
            Validator = new ExpressionValidator(ValidationMode.FullDiscreteConsume)
        };
    }

    // =============================== 固定 per-layout 逻辑 ===============================

    protected bool HandleLayoutsIndividually(
        GlobalDifficultyContext ctx,
        List<ExpressionLayout> layouts,
        out IEnumerable<GlobalDifficultyContext>? branches)
    {
        branches = null;
        bool modeProgress = false;

        var solveCtx = CreateSolveContext(ctx);

        foreach (var layout in layouts)
        {
            var origin = layout.ToExpression(ctx.Board);
            var solved = ctx.Solver.Solve(origin, solveCtx).ToList();

            if (solved.Count == 0) return false;

            var table = layout.BuildCandidateTable(origin, solved);
            if (table.Count == 0) return false;

            if (HandleCandidateTable(ctx, layout, table, out branches))
            {
                if (branches != null)
                    throw new InvalidOperationException("true + branches != null");

                modeProgress = true;
            }

            if (branches != null)
                return false; // 分支产生
        }

        return modeProgress;
    }
}
