namespace CrossMath.Core.Evaluation.GlobalCellDifficulty;
public interface IGlobalDifficultyLayer
{
    int Difficulty { get; }  

    /// <summary>
    /// 尝试在本上下文中应用当前技巧
    /// </summary>
    /// <returns>
    /// true  → 成功确定性地填了至少一个数，ctx 已被修改，branches 为 null
    /// false → 无法确定性填数：
    ///         - 如果 branches == null        → 本层完全无效
    ///         - 如果 branches 有值           → 需要猜数，已返回所有猜测分支（每个分支已填一个候选）
    /// </returns>
    bool TryEvaluate(
        GlobalDifficultyContext ctx,
        out IEnumerable<GlobalDifficultyContext>? branches);
}