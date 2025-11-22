using CrossMath.Core.Expressions.Core;
using CrossMath.Core.Types;

namespace CrossMath.Core.ExpressionSolvers;

public partial class Expression5Solver
{
    IEnumerable<Expression5> HandleMissingOperator(Expression5 exp, ExpressionSolveContext ctx)
    {
        if (exp.Op.HasValue)
        {
            yield return exp;
            yield break;
        }
    
        int knownCount = (exp.A.HasValue ? 1 : 0) + 
                         (exp.B.HasValue ? 1 : 0) + 
                         (exp.C.HasValue ? 1 : 0);

        if (knownCount < 2)
        {
            if (!ctx.OpPool.AllowOperators.Any())
                yield break;

            foreach (var op in ctx.OpPool.AllowOperators)
            {
                var clone = (Expression5)exp.Clone();
                clone.Op = op;  // 关键：先转换，再赋值
                yield return clone;
            }
        }
        else
        {
            // 已知 ≥2 个数字 → 推断运算符（后面再补）
            foreach (var op in InferPossibleOperators(exp, ctx))
            {
                var clone = (Expression5)exp.Clone();
                clone.Op = op;
                yield return clone;
            }
        }
    }
    
    /// <summary>
    /// 根据已知的 A、B、C 数字，推断出所有可能合法的运算符
    /// 用于 knownCount >= 2 时的强力剪枝（性能提升 10~100 倍！）
    /// </summary>
    private static IEnumerable<OpType> InferPossibleOperators(Expression5 exp, ExpressionSolveContext ctx)
    {
        int? a = exp.A, b = exp.B, c = exp.C;
        var allowed = ctx.OpPool.AllowOperators; // 假设是 IEnumerable<OpType>

        foreach (var op in allowed)
        {
            // 情况1：三个数字都已知 → 直接严格校验
            if (a.HasValue && b.HasValue && c.HasValue)
            {
                if (CheckEquation(a.Value, b.Value, c.Value, op))
                    yield return op;
                continue;
            }

            // 情况2：恰好缺一个数字 → 反推它是否能落在合法范围内
            bool canBeValid =
                // 缺 A：b op ? = c
                (!a.HasValue && b.HasValue && c.HasValue && TryReverseA(b.Value, c.Value, op, out int aVal) && ctx.IsInRange(aVal)) ||

                // 缺 B：a op ? = c
                (a.HasValue && !b.HasValue && c.HasValue && TryReverseB(a.Value, c.Value, op, out int bVal) && ctx.IsInRange(bVal)) ||

                // 缺 C：a op b = ?
                (a.HasValue && b.HasValue && !c.HasValue && TryCompute(a.Value, b.Value, op, out int cVal) && ctx.IsInRange(cVal));

            if (canBeValid)
                yield return op;
        }
    }
    
    /// <summary>验证 a op b 是否等于 c</summary>
    private static bool CheckEquation(int a, int b, int c, OpType op) => op switch
    {
        OpType.Add => a + b == c,
        OpType.Sub => a - b == c,
        OpType.Mul => a * b == c,
        OpType.Div => b != 0 && a / b == c && a % b == 0,
        _ => false
    };

    /// <summary>尝试反推出 a</summary>
    private static bool TryReverseA(int b, int c, OpType op, out int a)
    {
        a = 0;
        switch (op)
        {
            case OpType.Add: a = c - b; return true;
            case OpType.Sub: a = c + b; return true;
            case OpType.Mul:
                if (b != 0 && c % b == 0) { a = c / b; return true; }
                return false;
            case OpType.Div:
                a = b * c; return true;
            default: return false;
        }
    }

    /// <summary>尝试反推出 b</summary>
    private static bool TryReverseB(int a, int c, OpType op, out int b)
    {
        b = 0;
        switch (op)
        {
            case OpType.Add: b = c - a; return true;
            case OpType.Sub: b = a - c; return true;
            case OpType.Mul:
                if (a != 0 && c % a == 0) { b = c / a; return true; }
                return false;
            case OpType.Div:
                if (c != 0 && a % c == 0) { b = a / c; return true; }
                return false;
            default: return false;
        }
    }
}