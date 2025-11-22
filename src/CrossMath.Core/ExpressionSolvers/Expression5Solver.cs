using CrossMath.Core.Types;
using CrossMath.Core.Expressions.Core;
namespace CrossMath.Core.ExpressionSolvers;

public partial class Expression5Solver : IExpressionSolver
{
    public IEnumerable<IExpression> Solve(IExpression expression, ExpressionSolveContext ctx)
    {
        // ✅ 类型校验
        if (expression is not Expression5 expr)
            throw new ArgumentException("仅支持 Expression5 类型。");

        // ✅ 如果已经完全填满，直接验证是否合法
        if (expr.IsFullyFilled)
        {
            // Evaluate 内部应该已经处理了除零等非法情况，返回 false 表示不合法
            if (expr.Evaluate())                     // 假设 Evaluate() 返回 true 表示计算合法
            {
                yield return expr.Clone();            // 找到一个合法解
            }
            // 不合法直接结束
            yield break;
        }
        
        // 1. 处理缺失的运算符（可能是 null 或 OpType.None）
        IEnumerable<Expression5> candidates = HandleMissingOperator(expr, ctx);

        // 2. 对每一个候选表达式，继续按运算符分派求解
        foreach (var candidate in candidates)
        {
            // 已经是一个完整表达式，直接验证结果
            if (candidate.IsFullyFilled)
            {
                if (candidate.Evaluate())  // 包含除零检查
                {
                    yield return candidate.Clone();
                }
                continue; // 不继续往下分派
            }

            // 还没完整 → 根据当前根运算符继续分派专用求解器
            var subSolutions = candidate.Op! switch
            {
                OpType.Add => SolveAdd(candidate, ctx),
                OpType.Sub => SolveSub(candidate, ctx),
                OpType.Mul => SolveMul(candidate, ctx),
                OpType.Div => SolveDiv(candidate, ctx),
                _ => Enumerable.Empty<Expression5>()
            };

            foreach (var sol in subSolutions)
            {
                yield return sol;
            }
        }
    }


    IEnumerable<Expression5> SolveByOp(Expression5 exp, ExpressionSolveContext ctx, OpType op)
    {
        int? A = exp.A, B = exp.B, C = exp.C;
        
        var iter = ctx.NumPool.NumList.Count > 0
            ? FindTriples(ctx.NumPool.NumList, op, A, B, C)
            : FindTriples(ctx.NumPool.Min, ctx.NumPool.Max, op, A, B, C);
        
        // 使用统一的、确定性的三元组枚举器（你之前写的 FindTriples）
        foreach (var (a, b, c) in iter)
        {
            // 必须匹配已知的数字（关键！）
            if (A.HasValue && A.Value != a) continue;
            if (B.HasValue && B.Value != b) continue;
            if (C.HasValue && C.Value != c) continue;
            
            if (!A.HasValue && !ctx.IsInRange(a)) continue;
            if (!B.HasValue && !ctx.IsInRange(b)) continue;
            if (!C.HasValue && !ctx.IsInRange(c)) continue;
            // 构造新表达式
            var clone = (Expression5)exp.Clone();
            clone.A = a;
            clone.B = b;
            clone.C = c;
            clone.Op = op;

            yield return clone;
        }
    }
      
    

  public static IEnumerable<(int a, int b, int c)> FindTriples(
        int min, int max,
        OpType op,
        int? knownA = null,
        int? knownB = null,
        int? knownC = null)
    {
        var iter=  op switch  
        {  
            OpType.Add => AddTriples(),  
            OpType.Sub => SubTriples(),  
            OpType.Mul => MulTriples(),  
            OpType.Div => DivTriples(),  
            _ => Enumerable.Empty<(int a, int b, int c)>()  
        };    
        foreach (var result in iter)  
        {       
            yield return result;  
        }    
        yield break;  


        // ==================== 加法：从源头剪枝 ====================
        IEnumerable<(int a, int b, int c)> AddTriples()
        {
            foreach (int a in knownA.HasValue ? new[] { knownA.Value } : Range(min, max))
            {
                int bMin = knownB ?? min;
                int bMax = knownC.HasValue ? knownC.Value - a : max - a;
                bMax = Math.Min(bMax, max);
                if (bMax < bMin) continue;

                foreach (int b in knownB.HasValue ? new[] { knownB.Value } : Range(bMin, bMax))
                {
                    int c = a + b;
                    if (c > max || c < min) continue;
                    if (knownC.HasValue && c != knownC.Value) continue;

                    yield return (a, b, c);
                }
            }
        }

        // ==================== 减法：从源头剪枝 ====================
        IEnumerable<(int a, int b, int c)> SubTriples()
        {
            foreach (int a in knownA.HasValue ? new[] { knownA.Value } : Range(min, max))
            {
                int bMin = knownB ?? min;
                int bMax = knownC.HasValue ? a - knownC.Value : a - min;
                bMax = Math.Min(bMax, max);
                if (bMax < bMin) continue;

                foreach (int b in knownB.HasValue ? new[] { knownB.Value } : Range(bMin, bMax))
                {
                    int c = a - b;
                    if (c > max || c < min) continue;
                    if (knownC.HasValue && c != knownC.Value) continue;

                    yield return (a, b, c);
                }
            }
        }

        // ==================== 乘法：因子级剪枝 ====================
        IEnumerable<(int a, int b, int c)> MulTriples()
        {
            var aSource = knownA.HasValue ? new[] { knownA.Value } : Range(min, max);
            foreach (int a in aSource.Where(x => x != 0))
            {
                var bSource = knownB.HasValue
                    ? new[] { knownB.Value }
                    : knownC.HasValue
                        ? GetFactors(knownC.Value).Where(b => b >= min && b <= max && (long)a * b == knownC.Value)
                        : Range(Math.Max(min, -max / Math.Abs(a)), Math.Min(max, max / Math.Abs(a)));

                foreach (int b in bSource.Where(x => x != 0))
                {
                    long cVal = (long)a * b;
                    if (cVal < min || cVal > max) continue;
                    int c = (int)cVal;
                    if (knownC.HasValue && c != knownC.Value) continue;

                    yield return (a, b, c);
                }
            }
        }

        // ==================== 除法：宇宙最快！反向枚举 ====================
        IEnumerable<(int a, int b, int c)> DivTriples()
        {
            var cSource = knownC.HasValue ? new[] { knownC.Value } : Range(min, max);
            var bSource = knownB.HasValue ? new[] { knownB.Value } : Range(min, max);

            foreach (int c in cSource)
            foreach (int b in bSource)
            {
                if (b == 0) continue;
                long aVal = (long)c * b;
                if (aVal < min || aVal > max) continue;
                int a = (int)aVal;

                if (knownA.HasValue && a != knownA.Value) continue;

                yield return (a, b, c);
            }
        }

        // 辅助：快速 Range
        IEnumerable<int> Range(int start, int end)
        {
            for (int i = start; i <= end; i++) yield return i;
        }

        IEnumerable<int> GetFactors(int n)
        {
            int abs = Math.Abs(n);
            for (int i = 1; i * i <= abs; i++)
            {
                if (abs % i == 0)
                {
                    yield return i;
                    if (i != abs / i) yield return abs / i;
                    yield return -i;
                    if (i != abs / i) yield return -(abs / i);
                }
            }
        }
    }

    /// <summary>
    /// 在可用数字集合中，枚举所有满足 a ? b = c 的三元组
    /// 已知位置使用固定值，未知位置从集合枚举
    /// 完全符合填空题/24点/卡牌计算的真实逻辑
    /// </summary>
    public static IEnumerable<(int a, int b, int c)> FindTriples(
        IEnumerable<int> availableNumbers,
        OpType op,
        int? knownA = null,
        int? knownB = null,
        int? knownC = null)
    {
        var candidates = availableNumbers.Distinct().ToList();
        var lookup = new HashSet<int>(candidates);

        // 确定三个位置的实际枚举源
        IEnumerable<int> aSource = knownA.HasValue ? new[] { knownA.Value } : candidates;
        IEnumerable<int> bSource = knownB.HasValue ? new[] { knownB.Value } : candidates;

        foreach (int a in aSource)
        foreach (int b in bSource)
        {
            if (!TryCompute(a, b, op, out int c)) continue;

            // c 必须属于可用数字集合（除非 knownC 强制指定）
            bool cValid = knownC.HasValue 
                ? c == knownC.Value 
                : lookup.Contains(c);

            if (cValid)
                yield return (a, b, c);
        }
    }

    #region 工具方法

    public static IEnumerable<int> DivisibleNumbersInRange(int divisor, int min, int max)
    {
        if (divisor == 0 || min > max) yield break;

        long d = divisor;
        long absD = Math.Abs(d);

        // 计算起点：>= min 的第一个倍数
        long start = min + (d - min % d) % d;  // 魔法公式，神级！

        // 处理负数取模的特殊情况（C# 负数 % 正数 = 负数）
        if (min % d < 0) start += d;

        for (long x = start; x <= max; x += absD)
        {
            yield return (int)x;
        }
    }
    
    /// <summary>
    /// 返回 n 的所有正质因子（带重复），顺序从小到大
    /// 例如：PositivePrimeFactors(60) → 2, 2, 2, 3, 5
    /// 支持负数、处理 0 和 1，性能极高（6k±1 轮式）
    /// </summary>
    public static IEnumerable<int> PositivePrimeFactors(int n)
    {
        int m = Math.Abs(n);
        if (m < 2) yield break;

        // 1. 处理因子 2
        while ((m & 1) == 0)
        {
            yield return 2;
            m >>= 1;
        }

        // 2. 统一用 6k±1 轮式处理所有奇数（包括 3）
        // f 从 3 开始，步长 6，检查 f 和 f+2
        for (long f = 3; f * f <= m; f += 6)
        {
            // 检查 f (6k-1)
            while (m % f == 0)
            {
                yield return (int)f;
                m /= (int)f;
            }

            // 检查 f+2 (6k+1)
            long g = f + 2;
            if (g * g > m) break; // 提前退出，防止 g 溢出判断

            while (m % g == 0)
            {
                yield return (int)g;
                m /= (int)g;
            }
        }

        // 3. 剩余部分是一个大于 1 的质数
        if (m > 1)
            yield return m;
    }

    private static bool TryCompute(int a, int b, OpType op, out int c)
    {
        c = op switch
        {
            OpType.Add => a + b,
            OpType.Sub => a - b,
            OpType.Mul => a * b,
            OpType.Div => b != 0 && a % b == 0 ? a / b : int.MinValue,
            _ => int.MinValue
        };
        return c != int.MinValue;
    }
    
    /*
     * 1. 组合
     * 2. 选择合法的
     */
    
    #endregion
}