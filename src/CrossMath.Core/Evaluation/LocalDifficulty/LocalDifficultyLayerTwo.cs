using CrossMath.Core.Expressions.Layout;

namespace CrossMath.Core.Evaluation.LocalDifficulty;

public class LocalDifficultyLayerTwo : LocalDifficultyBase
{
    public override int Difficulty => 2;

    public override void Evaluate(LocalDifficultyContext ctx)
    {
        // 获取只有一个空格表达式
        var emptyTwoLayouts = ctx.Layouts
            .Where(expLayout => expLayout.EmptyCellCount(ctx.Board) == 2)
            .ToList();

        var manager = BuildCandidateManager(ctx, emptyTwoLayouts);
        AnalyzeCandidateManager(ctx, manager);
    }
}