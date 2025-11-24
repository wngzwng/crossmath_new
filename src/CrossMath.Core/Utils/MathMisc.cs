namespace CrossMath.Core.Utils;

/// <summary>
/// 高性能数学小工具：平方/立方判断、质数判断、因数/质因数枚举、区间整除枚举。
/// </summary>
public static class MathMisc
{
    /// <summary>判断 n 是否为完全平方数（n >= 0）。负数返回 false。</summary>
    public static bool IsPerfectSquare(int n)
    {
        if (n < 0) return false;
        int r = (int)Math.Sqrt(n);
        // 用 r 和 r+1 都验证一次，避免边界误差
        return (long)r * r == n || (long)(r + 1) * (r + 1) == n;
    }

    /// <summary>
    /// 判断 n 是否为完全立方数。负数允许，例如 -8 为立方数。
    /// </summary>
    public static bool IsPerfectCube(int n)
    {
#if NET7_0_OR_GREATER
        // .NET 7+ 有 Math.Cbrt，性能更好
        int r = (int)Math.Round(Math.Cbrt(n));
        long v = (long)r * r * r;
        if (v == n) return true;
        // 为保险再检查相邻整数（极少数边界）
        long v1 = (long)(r + 1) * (r + 1) * (r + 1);
        long v2 = (long)(r - 1) * (r - 1) * (r - 1);
        return v1 == n || v2 == n;
#else
        // 兼容更低版本：用二分搜索整数立方根
        int sign = n < 0 ? -1 : 1;
        long m = Math.Abs((long)n);
        int lo = 0, hi = (int)Math.Min(1291, Math.Ceiling(Math.Pow(m, 1.0 / 3) + 2)); // 1291^3 > int.Max
        while (lo <= hi)
        {
            int mid = (lo + hi) >> 1;
            long cube = (long)mid * mid * mid;
            if (cube == m) return true;
            if (cube < m) lo = mid + 1;
            else hi = mid - 1;
        }
        return false;
#endif
    }

    /// <summary>
    /// 判断 n 是否为质数（素数）。定义：n &gt;= 2。
    /// </summary>
    public static bool IsPrime(int n)
    {
        if (n < 2) return false;
        if ((n & 1) == 0) return n == 2;
        if (n % 3 == 0) return n == 3;

        // 6k±1 轮询到 sqrt(n)
        int limit = (int)Math.Sqrt(n);
        for (int i = 5; i <= limit; i += 6)
        {
            if (n % i == 0 || n % (i + 2) == 0) return false;
        }
        return true;
    }

    /// <summary>
    /// 返回 n 的正因数（升序）。n == 0 返回空；n &lt; 0 使用 |n|。
    /// </summary>
    public static IEnumerable<int> PositiveDivisors(int n)
    {
        if (n == 0) yield break;
        int m = Math.Abs(n);
        var small = new List<int>();
        var large = new List<int>();

        int root = (int)Math.Sqrt(m);
        for (int d = 1; d <= root; d++)
        {
            if (m % d == 0)
            {
                small.Add(d);
                int q = m / d;
                if (q != d) large.Add(q);
            }
        }

        // small 已升序，large 需逆序以保证整体升序
        foreach (var x in small) yield return x;
        for (int i = large.Count - 1; i >= 0; i--) yield return large[i];
    }

    /// <summary>
    /// 返回 n 的正质因数（升序，含重数）。n == 0 或 n == 1 返回空；n &lt; 0 使用 |n|。
    /// </summary>
    public static IEnumerable<int> PositivePrimeFactors(int n)
    {
        int m = Math.Abs(n);
        if (m < 2) yield break;

        // 先分解 2
        while ((m & 1) == 0)
        {
            yield return 2;
            m >>= 1;
        }
        // 再分解 3
        while (m % 3 == 0)
        {
            yield return 3;
            m /= 3;
        }

        // 6k±1 试除
        for (int f = 5; (long)f * f <= m; f += 6)
        {
            while (m % f == 0)
            {
                yield return f;
                m /= f;
            }
            int g = f + 2;
            while (m % g == 0)
            {
                yield return g;
                m /= g;
            }
        }

        // 剩余一个大于 1 的因子（必为质数）
        if (m > 1) yield return m;
    }

    /// <summary>
    /// 在闭区间 [min, max] 内，按升序返回所有可被 divisor 整除的整数。
    /// divisor == 0 时返回空（数学上不可定义）。
    /// </summary>
    public static IEnumerable<int> DivisibleNumbersInRange(int divisor, int min, int max)
    {
        if (min > max) yield break;
        int step = Math.Abs(divisor);
        if (step == 0) yield break;

        // 计算第一个 >= min 的倍数
        int start = AlignToNextMultiple(min, step);
        // 保护：start 可能溢出或越界，转 long 迭代
        for (long x = start; x <= max; x += step)
            yield return (int)x;
    }

    // ========== 内部辅助 ==========

    /// <summary>
    /// 将 x 对齐到不小于 x 的最小 step 倍数（step &gt; 0）。
    /// </summary>
    private static int AlignToNextMultiple(int x, int step)
    {
        // 使余数为正：((x % step) + step) % step
        int r = x % step;
        if (r < 0) r += step; // 等价于 (x % step + step) % step
        return r == 0 ? x : SafeAdd(x, step - r);
    }

    /// <summary>
    /// 安全整型加法（溢出时钳制到 int.MaxValue / int.MinValue）。
    /// </summary>
    private static int SafeAdd(int a, int b)
    {
        long s = (long)a + b;
        if (s > int.MaxValue) return int.MaxValue;
        if (s < int.MinValue) return int.MinValue;
        return (int)s;
    }
}

