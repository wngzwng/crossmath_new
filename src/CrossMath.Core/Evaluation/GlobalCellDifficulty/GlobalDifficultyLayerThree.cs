using CrossMath.Core.CandidateDomains;
using CrossMath.Core.CSP;
using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.ExpressionSolvers.SolverProviders;
using CrossMath.Core.Models;
using CrossMath.Core.Types;
using CrossMath.Core.Utils;

namespace CrossMath.Core.Evaluation.GlobalCellDifficulty;

public class GlobalDifficultyLayerThree: GlobalDifficultyLayerBase
{
    public override int Difficulty => 3;
    protected override bool UsePerLayoutProcessing => false;
    
    private CrossMathCSP _csp = new CrossMathCSP();
    protected override List<ExpressionLayout> ExtractTargetLayouts(BoardData board, GlobalDifficultyContext ctx)
    {
        /*
         * return ExpressionLayoutBuilder
           .ExtractLayouts(board.Layout, [5,7])
           .Where(l => l.EmptyNumberCount(board) == 1 && !l.HasEmptySymbol(board))
           .ToList();
         */
        var explayouts = ExpressionLayoutBuilder.ExtractLayouts(ctx.Board.Layout, [5, 7]);
        var oneNumberHoleLayouts =
            explayouts.Where(lay => lay.EmptyCellCount(board) > 0)
                .ToList();
        return oneNumberHoleLayouts;
    }
    
    protected override bool HandleLayoutsBatch(
        GlobalDifficultyContext ctx,
        List<ExpressionLayout> layouts,
        out IEnumerable<GlobalDifficultyContext>? branches)
    {
        branches = null;

        if (!BuildCandidateDomainManager(ctx, layouts, out var domainManager))
            return false;

        // 检查直接的单候选域表
        if (TryProcessSingleCandidateTables(ctx, domainManager))
            return true;

        // 执行CSP传播并处理结果
        var cloneManager = domainManager.Clone();
        var cspResult = _csp.RunPropagation(domainManager, ctx.Board.PossibleAnswers);

        // 处理CSP传播后产生的单候选域
        if (TryProcessSingleCandidateVariables(ctx, cspResult.VariableDomains))
            return true;

        var candidates = cspResult.VariableDomains
            .Where(kvp => kvp.Value.Count > 1) // 只考虑多候选域的变量
            .ToList();

        if (candidates.Count == 0)
        {
            return false;  // 没有分支，无需继续推进
        }
        
        // 检查是否需要创建分支
        var minDomainVariable = candidates.MinBy(kvp => kvp.Value.Count);
        if (minDomainVariable.Value.Count > 1)
        {
            branches = CreateBranches(ctx, minDomainVariable.Key, minDomainVariable.Value);
            return false;  // 需要分支搜索
        }

        return false;  // 无法进一步确定，也不需要分支
    }

    protected bool BuildCandidateDomainManager(
        GlobalDifficultyContext ctx,
        List<ExpressionLayout> layouts, out CandidateDomainManager<RowCol, string> manager)
    {
        var solveCtx = CreateSolveContext(ctx);
        manager = new CandidateDomainManager<RowCol, string>();
        foreach (var layout in layouts)
        {
            var origin = layout.ToExpression(ctx.Board);
            var solved = ctx.Solver.Solve(origin, solveCtx).ToList();

            if (solved.Count == 0) return false;

            var table = layout.BuildCandidateTable(origin, solved);
            if (table.Count == 0) continue;
            
            manager.Add(layout.ToString(), table);
        }
        
        return true;
    }
    
    private bool TryProcessSingleCandidateTables(GlobalDifficultyContext ctx, CandidateDomainManager<RowCol, string> domainManager)
    {
        foreach (var table in domainManager.Tables)
        {
            if (table.IsSingle)
            {
                var map = table[0].ToDictionary();
                var counts = CounterUtils.CountValues(ctx.Board.PossibleAnswers);
            
                if (!CounterUtils.IsValidMultiset(counts, map.Values.ToArray()))
                    return false;

                ctx.UpdateDifficulty(Difficulty);
                AssignValues(ctx, map);
                return true;
            }
        }
        return false;
    }
    
    
    private bool TryProcessSingleCandidateVariables(GlobalDifficultyContext ctx, Dictionary<RowCol, HashSet<string>> variableDomains)
    {
        var singleCandidateVariables = variableDomains
            .Where(kvp => kvp.Value.Count == 1)
            .ToList();

        if (!singleCandidateVariables.Any())
            return false;

        ctx.UpdateDifficulty(Difficulty);
        foreach (var variable in singleCandidateVariables)
        {
            var pos = variable.Key;
            var value = variable.Value.Single();
        
            AssignSingleValue(ctx, pos, value);
        }

        return true;
    }
    
    private void AssignValues(GlobalDifficultyContext ctx, Dictionary<RowCol, string> valueMap)
    {
        foreach (var kvp in valueMap)
        {
            AssignSingleValue(ctx, kvp.Key, kvp.Value);
        }
    }

    private void AssignSingleValue(GlobalDifficultyContext ctx, RowCol position, string value)
    {
        ctx.Board.SetValueOnly(position, value);
        ctx.Board.RemovePossibleAnswer(value);
        ctx.DifficultyRecord[position] = ctx.CurrentDifficulty;
    }
    
    // 提取分支创建逻辑
    private IEnumerable<GlobalDifficultyContext> CreateBranches(
        GlobalDifficultyContext ctx, 
        RowCol position, 
        IEnumerable<string> candidateValues)
    {
        var branches = new List<GlobalDifficultyContext>();
        
        foreach (var value in candidateValues)
        {
            var newCtx = ctx.Clone();
            newCtx.UpdateDifficulty(4);
        
            newCtx.Board.SetValueOnly(position, value);
            newCtx.Board.RemovePossibleAnswer(value);
            newCtx.DifficultyRecord[position] = newCtx.CurrentDifficulty;

            branches.Add(newCtx);
        }
        return branches;
    }

}