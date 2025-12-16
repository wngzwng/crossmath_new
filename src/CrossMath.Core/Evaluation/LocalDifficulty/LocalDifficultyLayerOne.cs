using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.ExpressionSolvers;
using CrossMath.Core.ExpressionSolvers.ExpressionValidators;
using CrossMath.Core.ExpressionSolvers.NumberPools;
using CrossMath.Core.ExpressionSolvers.OperatorPools;
using CrossMath.Core.ExpressionSolvers.SolverProviders;
using CrossMath.Core.Types;

namespace CrossMath.Core.Evaluation.LocalDifficulty;

public class LocalDifficultyLayerOne: LocalDifficultyBase
{
    public override int Difficulty => 1;

    protected override ExpressionSolveContext CreateSolveContext(LocalDifficultyContext ctx)
    {
        /*
        难度1: 不需要依靠盘面候选数的限制，所以这里比较宽松
         */
        return new ExpressionSolveContext()
        {
            NumPool = NumberPoolFactory.Create(1, 240, NumberOrder.Ascending),
            OpPool = OperatorPoolFactory.ASMD,
            Validator = new ExpressionValidator(ValidationMode.FullPoolCheck)
        };
    }
    public override void Evaluate(LocalDifficultyContext ctx)
    {
        // 获取只有一个空格表达式
        var emptyOneLayouts = ctx.Layouts
            .Where(expLayout => expLayout.EmptyCellCount(ctx.Board) == 1)
            .ToList();

        var manager = BuildCandidateManager(ctx, emptyOneLayouts);
        AnalyzeCandidateManager(ctx, manager);
    }
}   