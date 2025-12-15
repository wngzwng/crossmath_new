using CrossMath.Core.Expressions.Core;
using CrossMath.Core.Expressions.Schema;
using CrossMath.Core.Models;
using CrossMath.Core.Types;
using CrossMath.Core.Utils;

namespace CrossMath.Core.Expressions;

public static class WeakExpressionUtils
{
    /// <summary>
    /// 判断表达式是否包含弱算式（×1 或 ÷1）
    /// </summary>
    public static bool ContainsMulDivByOne(IExpression expr)
    {
        // var tokens = expr.GetTokens();
        // return tokens.Contains("1") && (tokens.Contains(SymbolManager.SymbolMul) || tokens.Contains(SymbolManager.SymbolDiv));
        
        // 操作符均在奇数位，最后一个操作符索引 = Length - 2
        for (int i = 1; i < expr.Length - 1; i += 2)
        {
            string op = expr[i];
            if (op != SymbolManager.SymbolMul && op != SymbolManager.SymbolDiv)
                continue;

            string left = expr[i - 1];
            string right = expr[i + 1];

            if (left == "1" || right == "1")
                return true;
        }

        return false;
    }

    
    public static bool WeakEqual(IExpression a, IExpression b)
    {
        if (a.Length != b.Length)
            return false;

        if (!a.IsFullyFilled || !b.IsFullyFilled)
            return false;

        var countA = CounterUtils.CountValues(a.GetTokens());
        var countB = CounterUtils.CountValues(b.GetTokens());

        if (countA.Count != countB.Count)
            return false;

        foreach (var (key, value) in countA)
        {
            if (!countB.TryGetValue(key, out int cnt) || cnt != value)
                return false;
        }

        return true;
    }

    public static string GetWeakId(IExpression expr)
    {
        if (!expr.IsFullyFilled)
            throw new ArgumentException("Expression not fully filled.", nameof(expr));

        List<string> values = new();
        List<string> ops = new();

        // 偶数位置是值，奇数位置是运算符（5/7 长度表达式的固定结构）
        var schema = ExpressionSchemaFactory.Create(expr.Length);
        for (int i = 0; i < expr.Length; i++)
        {
            var cellType = schema.GetCellType(i);
            if (cellType == CellType.Operator) ops.Add(expr[i]);
            if (cellType == CellType.Number) values.Add(expr[i]);
        }

        values.Sort(StringComparer.Ordinal);
        ops.Sort(StringComparer.Ordinal);

        // 组装稳定 ID
        return $"{string.Join(",", values)}|{string.Join(",", ops)}";
    }
}
