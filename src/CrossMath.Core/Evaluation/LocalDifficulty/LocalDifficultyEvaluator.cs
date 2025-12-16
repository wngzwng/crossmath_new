using CrossMath.Core.Evaluation.GlobalCellDifficulty;
using CrossMath.Core.ExpressionSolvers.SolverProviders;
using CrossMath.Core.Models;
using CrossMath.Core.Types;
using Microsoft.Extensions.Logging;

namespace CrossMath.Core.Evaluation.LocalDifficulty;

public class LocalDifficultyEvaluator
{
    private readonly IReadOnlyList<ILocalDifficultyLayer> _layers;
    
    private readonly ILogger _logger;
    public LocalDifficultyEvaluator(
        IEnumerable<ILocalDifficultyLayer> layers, 
        ILoggerFactory loggerFactory)
    {
        _layers = layers.OrderBy(l => l.Difficulty).ToList();
        _logger = loggerFactory.CreateLogger<LocalDifficultyEvaluator>();
    }

    public static LocalDifficultyEvaluator CreatorDefault(ILoggerFactory loggerFactory)
    {
        return new LocalDifficultyEvaluator(
            [
                new LocalDifficultyLayerOne(),
                new LocalDifficultyLayerTwo(),
                new LocalDifficultyThree(),
                new LocalDifficultyFour()
            ],
            loggerFactory);
    }
    
    public static LocalDifficultyContext CreateContext(BoardData board)
    {
        return new LocalDifficultyContext(
            board: board,
            solver: ExpressionSolverProvider.CreateDefault()
        );
    }

    public Dictionary<RowCol, int> Evaluate(LocalDifficultyContext context)
    {
        context.Reset();
        
        foreach (var layer in _layers)
        {
            if (layer.CanEvaluate(context))
            {
                layer.Evaluate(context);
            }
        }
        
        return context.Result.MinDifficultyPerCell.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    }
}