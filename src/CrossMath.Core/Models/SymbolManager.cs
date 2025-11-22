using CrossMath.Core.Types;
namespace CrossMath.Core.Models;

public static class SymbolManager
{
    // ==============================================================
    // ① 基础符号常量（唯一数据来源，无魔法字符串）
    // ==============================================================
    public const string SymbolAdd = "+";
    public const string SymbolSub = "-";
    public const string SymbolMul = "*";
    public const string SymbolDiv = "/";
    public const string SymbolEqual = "=";

    public static readonly HashSet<string> AllOperatorSymbols = new()
    {
        SymbolAdd, SymbolSub, SymbolMul, SymbolDiv
    };

    public static readonly HashSet<string> AllValidSymbols = new()
    {
        SymbolAdd, SymbolSub, SymbolMul, SymbolDiv, SymbolEqual
    };

    // ==============================================================
    // ② OpType ↔ 符号 的双射（Bijection）
    // ==============================================================
    private static readonly Dictionary<OpType, string> OpToSymbol = new()
    {
        [OpType.Add] = SymbolAdd,
        [OpType.Sub] = SymbolSub,
        [OpType.Mul] = SymbolMul,
        [OpType.Div] = SymbolDiv,
    };

    private static readonly Dictionary<string, OpType> SymbolToOp =
        OpToSymbol.ToDictionary(kv => kv.Value, kv => kv.Key);


    public static bool TryGetSymbol(OpType op, out string symbol)
        => OpToSymbol.TryGetValue(op, out symbol!); 
    
    public static string GetSymbolOrDefault(OpType? op, string? defaultVal)
    {
        return op.HasValue && TryGetSymbol(op.Value, out var sym) ? sym : (defaultVal ?? String.Empty);
    }

    public static bool TryGetOpType(string symbol, out OpType op)
        => SymbolToOp.TryGetValue(symbol, out op!);

    public static bool IsOpSymbol(string s) => SymbolToOp.ContainsKey(s);

    
    // ==============================================================
    // ④ Token 判断：是否符号编码？是否运算符？是否等号？
    // ==============================================================

    /// <summary>是否为符号本体（"+" "-" "*" "/" "="）</summary>
    public static bool IsSymbol(string token)
        => AllValidSymbols.Contains(token);

    /// <summary>是否为运算符符号</summary>
    public static bool IsOperatorToken(string token)
        => AllOperatorSymbols.Contains(token);

    /// <summary>是否为等号符号</summary>
    public static bool IsEqualToken(string token)
        => token == SymbolEqual;
    
    // ==============================================================
    // ⑤ CellType 推导（非法 ≠ 未指定）（用于挖空后的 HoleType）
    // ==============================================================
    public static CellType InferCellType(string val)
    {
        // 空值 → 未指定（用于空盘阶段）
        if (string.IsNullOrWhiteSpace(val))
            return CellType.Unspecified;

        // 数字（正整数）
        if (int.TryParse(val, out int num) && num > 0)
            return CellType.Number;

        // 运算符
        if (AllOperatorSymbols.Contains(val))
            return CellType.Operator;

        // 等号
        if (val == SymbolEqual)
            return CellType.Equal;

        // 其余 → 非法
        return CellType.Invalid;
    }

    /// <summary>推导操作符（失败→Unspecified）</summary>
    public static OpType InferOpType(string val)
        => SymbolToOp.GetValueOrDefault(val, OpType.Invalid);
}


