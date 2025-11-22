using CrossMath.Core.Types;
using CrossMath.Core.Expressions.Core;
using CrossMath.Core.Expressions.Layout;
namespace CrossMath.Core.ExpressionSolvers;

/// <summary>
/// 单个算式的完整候选组管理器（基于合法解集合）
/// </summary>
public sealed class ExpressionCandidateGroup
{
    // 1. 关联的算式布局
    public ExpressionLayout ExprLayout { get; }

    // 2. 所有合法的完整解（核心！）
    private IReadOnlyList<IExpression> Solutions { get; }

    // 3. 空白格子的位置（缓存，性能拉满）
    public IReadOnlyList<RowCol> BlankPositions { get; }

    // 当前存活的解索引（支持动态筛选）
    private readonly List<int> _aliveIndices;

    public int RemainingCount => _aliveIndices.Count;
    public bool IsSolved => RemainingCount == 1;
    public bool IsDead => RemainingCount == 0;

    // 构造函数：传入布局 + 合法解列表
    public ExpressionCandidateGroup(ExpressionLayout layout, IReadOnlyList<RowCol> blankPositions, IEnumerable<IExpression> solutions)
    {
        ExprLayout = layout ?? throw new ArgumentNullException(nameof(layout));
        Solutions = solutions?.ToArray() ?? throw new ArgumentNullException(nameof(solutions));

        if (Solutions.Count == 0)
            throw new ArgumentException("至少需要一个合法解");

        BlankPositions = blankPositions;

        _aliveIndices = Enumerable.Range(0, Solutions.Count).ToList();
    }

    // ==================== 核心 API ====================

    /// <summary>
    /// 获取某个位置当前可能的候选数字（动态投影）
    /// </summary>
    public IReadOnlyList<string> GetCandidates(RowCol position)
    {
        if (!ExprLayout.TryGetIndex(position, out int index))
        {
            return Array.Empty<string>();
        }
       
        var set = new HashSet<string>();
        foreach (int i in _aliveIndices)
        {
            var token = Solutions[i][index];
            if (! string.IsNullOrEmpty(token))
                set.Add(token);
        }
        return set.OrderBy(x => x).ToArray();
    }

    /// <summary>
    /// 筛选：某个位置已确定为某个数字 → 保留只包含该数字的解
    /// </summary>
    public ExpressionCandidateGroup Fix(RowCol position, string val)
    {
        if (!ExprLayout.TryGetIndex(position, out int index))
        {
            return this;
        }

        val = val.Trim();
        var newAlive = _aliveIndices
            .Where(i => Solutions[i][index] == val)
            .ToList();

        return WithNewAlive(newAlive);
    }

    /// <summary>
    /// 范围筛选：所有空白位必须出现在给定的候选池中（支持重复，如 [2,2,3]）
    /// </summary>
    public ExpressionCandidateGroup RestrictByPool(IReadOnlyList<int> candidatePool)
    {
        if (candidatePool.Count != BlankPositions.Count)
            throw new ArgumentException("候选池长度必须等于空白格子数");

        var newAlive = new List<int>();

        foreach (int i in _aliveIndices)
        {
            bool ok = true;
            for (int j = 0; j < BlankPositions.Count; j++)
            {
                // if (Solutions[i].Tokens[Layout.Cells.IndexOf(BlankPositions[j])] is NumberToken num)
                // {
                //     if (!candidatePool.Contains(num.Value))
                //     {
                //         ok = false;
                //         break;
                //     }
                // }
            }
            if (ok) newAlive.Add(i);
        }

        return WithNewAlive(newAlive);
    }

    /// <summary>
    /// 获取当前所有存活解（用于最终答案）
    /// </summary>
    public IReadOnlyList<IExpression> GetRemainingSolutions()
        => _aliveIndices.Select(i => Solutions[i]).ToArray();

    // 私有：返回新实例（不可变风格）
    private ExpressionCandidateGroup WithNewAlive(List<int> newAlive)
    {
        if (newAlive.Count == _aliveIndices.Count)
            return this; // 无变化

        var copy = new ExpressionCandidateGroup(ExprLayout, BlankPositions, Solutions)
        {
            // _aliveIndices = newAlive
        };
        return copy;
    }
}