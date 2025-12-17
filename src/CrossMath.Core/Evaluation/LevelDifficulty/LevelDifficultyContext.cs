using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Evaluation.LevelDifficulty;

public sealed class LevelDifficultyContext
{
   public BoardData InitialBoard { get; }
   public BoardData WorkingBoard { get; private set; }

    // 固定结构缓存（不随步骤变化）
    public IReadOnlyList<ExpressionLayout> Layouts { get; }

    // —— 运行期状态 ——
    public double Score { get; private set; }
    public int StepIndex { get; private set; }
    
    public RowCol? LastCoordinate {get; private set;}


    public LevelDifficultyContext(BoardData board)
    {
        InitialBoard = board.Clone(); // Level 模拟必须独立
        WorkingBoard = board.Clone();
        Layouts = ExpressionLayoutBuilder
            .ExtractLayouts(board.Layout, new[] { 5, 7 })
            .ToList();
        StepIndex = 0;
    }
    
    public bool IsComplete()
    {
        return WorkingBoard.Holes.Count == 0;
    }
    
    public void ApplySelection(RowCol coord, double score)
    {
        Score += score;
        StepIndex++;
        LastCoordinate  = coord;
        WorkingBoard.ApplyAnswer(coord, out _);
    }

    public void Reset()
    {
        Score = 0;
        StepIndex = 0;
        WorkingBoard = InitialBoard.Clone();
    }
}
