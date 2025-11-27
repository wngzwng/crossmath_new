using CrossMath.Core.CandidateDomains;
using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.ExpressionSolvers;
using CrossMath.Core.ExpressionSolvers.ExpressionValidators;
using CrossMath.Core.ExpressionSolvers.NumberPools;
using CrossMath.Core.ExpressionSolvers.OperatorPools;
using CrossMath.Core.Models;
using CrossMath.Core.Types;
using CrossMath.Core.Utils;

namespace CrossMath.Core.Evaluation.GobalCellDifficulty;

public class GlobalDifficultyLayerOne: GlobalDifficultyLayerBase
{
    public override int Difficulty => 1;
    protected override bool UsePerLayoutProcessing => true;
    
    protected override ExpressionSolveContext CreateSolveContext(GlobalDifficultyContext ctx)
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
    
    protected override List<ExpressionLayout> ExtractTargetLayouts(BoardData board, GlobalDifficultyContext ctx)
    {
        var explayouts = ExpressionLayoutBuilder.ExtractLayouts(ctx.Board.Layout, [5, 7]);
        var oneNumberHoleLayouts =
            explayouts.Where(lay =>
                    lay.EmptyNumberCount(ctx.Board) == 1)
                .ToList();
        return oneNumberHoleLayouts;
    }
}