using CrossMath.Core.Utils;

namespace CrossMath.Core.Analytics.utils;

/// <summary>
/// 数字友好度计算器（int 版）。
/// 计算方式：对每个候选值按规则给分，最终返回平均友好度。<br/>
/// <br/>
/// 评分规则：
/// <list type="bullet">
///   <item><description>+5：尾数为 00</description></item>
///   <item><description>+4：所有数字相同（如 8、33、111）</description></item>
///   <item><description>+3：5 的整数倍</description></item>
///   <item><description>+2：质因数仅包含 2、3</description></item>
///   <item><description>+2：完全平方数</description></item>
///   <item><description>+1：完全立方数</description></item>
/// </list>
/// <br/>
/// 负向规则：
/// <list type="bullet">
///   <item><description>-3：（旧）质因数都大于 10（已弃用，2025-05-21 修改）</description></item>
///   <item><description>-3：存在任意质因数 &gt; 10（新规则）</description></item>
/// </list>
/// <br/>
/// 输出：候选值友好度的平均值。
/// </summary>
public static class NumberFriendlyCalculator
{
    /// <summary>
    /// 计算候选值友好度平均值（整数版，返回 double 仅用于保存小数位）
    /// </summary>
    public static double CalcFriendly(IEnumerable<int> numbers)
    {
        var list = numbers.ToList();
        if (list.Count == 0)
            return 0;

        var scores = list.Select(x => MatchRule(x, RuleTable, 0)).ToList();

        return Math.Round(scores.Average(), 2);
    }

    // ======================================================
    // 规则表（全部 int）
    // ======================================================
    public static readonly List<(int Score, Func<int, bool> Cond)> RuleTable =
    [
        // 尾数 00
        (5, x => x % 100 == 0),

        // 数字全相同：8, 33, 111, 222
        (4, x => IsAllDigitsSame(x)),

        // 5 的倍数
        (3, x => x % 5 == 0),

        // 质因数仅有 2 和 3
        (2, x => PrimeFactors(x).ToHashSet().SetEquals([2, 3])),

        // 平方数
        (2, x => IsSquare(x)),

        // 立方数
        (1, x => IsCube(x)),

        // 存在质因数 > 10
        (-3, x => PrimeFactors(x).Any(f => f > 10)),
    ];

    // ======================================================
    // 规则匹配
    // ======================================================
    public static int MatchRule(int x,
        List<(int Score, Func<int, bool> Cond)> table,
        int defaultValue)
    {
        foreach (var (score, cond) in table)
        {
            if (cond(x))
                return score;
        }
        return defaultValue;
    }

    // ======================================================
    // 工具：检查全相同数字
    // ======================================================
    private static bool IsAllDigitsSame(int x)
    {
        var s = x.ToString();
        return s.All(c => c == s[0]);
    }

    // ======================================================
    // 平方数（纯 int 实现）
    // ======================================================
    private static bool IsSquare(int x)
    {
        return MathMisc.IsPerfectSquare(x);
        // if (x < 0) return false;
        //
        // int r = (int)Math.Sqrt(x);
        // return r * r == x;
    }

    // ======================================================
    // 立方数（纯 int 实现）
    // ======================================================
    private static bool IsCube(int x)
    {
        return MathMisc.IsPerfectCube(x);
        // if (x < 0) return false;
        //
        // int r = (int)Math.Round(Math.Pow(x, 1.0 / 3.0));
        // return r * r * r == x;
    }

    // ======================================================
    // 质因数分解（int 实现）
    // ======================================================
    private static IEnumerable<int> PrimeFactors(int x)
    {
        return MathMisc.PositivePrimeFactors(x);
        // if (x <= 1) yield break;
        //
        // int n = x;
        //
        // for (int f = 2; f * f <= n; f++)
        // {
        //     while (n % f == 0)
        //     {
        //         yield return f;
        //         n /= f;
        //     }
        // }
        //
        // if (n > 1)
        //     yield return n;
    }
}
