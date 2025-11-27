using CrossMath.Core.CandidateDomains;
using CrossMath.Core.ExpressionSolvers.SolverProviders;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Evaluation;

public sealed class GlobalDifficultyContext
{
    public CandidateDomainManager<RowCol,string> Manager { get; private set; }
    public int CurrentDifficulty { get; private set; }           // 当前已使用的最难技巧级别
    public BoardData Board { get; }                                  // 只读，求解过程中会修改
    public ExpressionSolverProvider Solver { get; }
    public Dictionary<RowCol, int> DifficultyRecord { get; private set; }     // 记录每步用了哪个难度

    public GlobalDifficultyContext(
        CandidateDomainManager<RowCol,string> manager,
        BoardData board,
        ExpressionSolverProvider solver)
    {
        Manager = manager;
        Board = board;                   // 注意：这里通常要 Clone 一份，避免影响原盘
        Solver = solver;
        CurrentDifficulty = 0;
        DifficultyRecord = new Dictionary<RowCol, int>();
    }

    // 深拷贝（关键！否则所有分支共享同一 Board 和 Manager）
    public GlobalDifficultyContext Clone()
    {
        return new GlobalDifficultyContext(
            Manager.Clone(),           // 必须深拷贝
            Board.Clone(),             // 必须深拷贝
            Solver)
        {
            CurrentDifficulty = this.CurrentDifficulty,
            DifficultyRecord = new Dictionary<RowCol, int>(this.DifficultyRecord)  // 假设 Difficulty 有 Clone
        };
    }

    public void UpdateDifficulty(int layerDifficulty)
    {
        if (layerDifficulty > CurrentDifficulty)
            CurrentDifficulty = layerDifficulty;
    }
}
