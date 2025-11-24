using CrossMath.Core.Types;

namespace CrossMath.Core.ExpressionSolvers.OperatorPools;

public static class OperatorPoolExtensions
{
    /// <summary>
    /// 验证一组数字是否是当前池子的有效多重子集（每个数字拿的个数不能超过池子里原有的）
    /// 支持 params 写法：pool.IsValidMultiset(1, 1, 3)
    /// </summary>
    public static bool IsValidMultiset(this IOperatorPool pool, params OpType[] ops)
        => pool.IsValidMultiset((IEnumerable<OpType>)ops);

    /// <summary>
    /// 核心实现：支持任意 IEnumerable<int>
    /// 完全不依赖 GetCount，只用 AllNumbers 统计原始次数
    /// </summary>
    /// <summary>
    /// 验证一组数字是否是当前池子的「有效多重子集」（支持 IEnumerable）
    /// </summary>
    public static bool IsValidMultiset(this IOperatorPool opPool,IEnumerable<OpType> ops)
    {
        var counter = new Dictionary<OpType, int>();
        foreach (var n in ops)
            counter[n] = counter.GetValueOrDefault(n) + 1;

        foreach (var kvp in counter)
        {
            if (kvp.Value > opPool.GetCount(kvp.Key))   // 抽的次数 > 池子里原本有的次数
                return false;
        }
        return true;
    }
}