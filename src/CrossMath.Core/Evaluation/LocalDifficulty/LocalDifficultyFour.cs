namespace CrossMath.Core.Evaluation.LocalDifficulty;

public class LocalDifficultyFour: LocalDifficultyBase
{
    public override int Difficulty => 4;
    
    public override void Evaluate(LocalDifficultyContext ctx)
    {
        // 求所有表达式的的候选组，然后交叉约束传播后的结果
        foreach (var rowCol in ctx.Board.Holes)
        {
            ctx.Result.MarkDifficulty(rowCol, 4);
        }
    }
}