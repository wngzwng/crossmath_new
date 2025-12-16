using CrossMath.Core.CandidateDomains;
using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.ExpressionSolvers;
using CrossMath.Core.ExpressionSolvers.ExpressionValidators;
using CrossMath.Core.ExpressionSolvers.NumberPools;
using CrossMath.Core.ExpressionSolvers.OperatorPools;
using CrossMath.Core.Models;
using CrossMath.Core.Types;
using CrossMath.Core.Utils;

namespace CrossMath.Core.Evaluation.GlobalCellDifficulty;

public class GlobalDifficultyLayerTwo: GlobalDifficultyLayerBase
{
    public override int Difficulty => 2;
    protected override bool UsePerLayoutProcessing => true;
    
    protected override List<ExpressionLayout> ExtractTargetLayouts(BoardData board, GlobalDifficultyContext ctx)
    {
        var explayouts = ExpressionLayoutBuilder.ExtractLayouts(ctx.Board.Layout, [5, 7]);
        var twoNumberHoleLayouts =
            explayouts.Where(lay =>
                    lay.EmptyNumberCount(ctx.Board) == 2)
                .ToList();
        return twoNumberHoleLayouts;
    }
}