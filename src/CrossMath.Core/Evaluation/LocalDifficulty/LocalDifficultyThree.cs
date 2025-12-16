using CrossMath.Core.CSP;

namespace CrossMath.Core.Evaluation.LocalDifficulty;

public class LocalDifficultyThree : LocalDifficultyBase
{
    public override int Difficulty => 3;
    
    private CrossMathCSP _csp = new CrossMathCSP();
    public override void Evaluate(LocalDifficultyContext ctx)
    {
        // 求所有表达式的的候选组，然后交叉约束传播后的结果
        var manager = BuildCandidateManager(ctx, ctx.Layouts);
        var cspResult = _csp.RunPropagation(manager, ctx.Board.PossibleAnswers);
        AnalyzeCandidateManager(ctx, cspResult.ExpressionCandidates);
    }
}