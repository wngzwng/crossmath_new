using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.ExpressionSolvers.SolverProviders;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Evaluation.LocalDifficulty;

public sealed class LocalDifficultyContext
{
    public BoardData Board { get; }
    
    /// <summary>
    /// Pre-extracted expression layouts derived from Board layout.
    /// This is a structural cache shared by all local difficulty layers.
    /// </summary>
    public IReadOnlyList<ExpressionLayout> Layouts { get; }
    public ExpressionSolverProvider Solver { get; }
    public LocalEvaluationResult Result { get; } = new();
    
    public Dictionary<RowCol, HashSet<string>> CandidateMapAtCell { get; } = new ();

    public LocalDifficultyContext(BoardData board, ExpressionSolverProvider solver)
    {
        Board = board;
        Solver = solver;
        Layouts = ExpressionLayoutBuilder.ExtractLayouts(board.Layout, [5, 7]).ToList();
    }
    
    public void Reset()
    {
        Result.Reset();
        CandidateMapAtCell.Clear();
    }

}