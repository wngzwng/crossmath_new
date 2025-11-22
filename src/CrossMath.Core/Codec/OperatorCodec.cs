using CrossMath.Core.Models;
namespace CrossMath.Core.Codec;

public static class OperatorCodec
{
    // ==============================================================
    // ③ 编码（Hex）双射：符号 ↔ Hex（fa, fb, fc...）
    // ==============================================================
    private static readonly Dictionary<string, string> EncodeMap = new()
    {
        [SymbolManager.SymbolEqual] = "fa",  // "="
        [SymbolManager.SymbolAdd]   = "fb",  // "+"
        [SymbolManager.SymbolSub]   = "fc",  // "-"
        [SymbolManager.SymbolMul]   = "fd",  // "*"
        [SymbolManager.SymbolDiv]   = "fe",  // "/"
    };

    private static readonly Dictionary<string, string> DecodeMap =
        EncodeMap.ToDictionary(kv => kv.Value, kv => kv.Key);
 
    public const string NumberHoleHex = "00"; // 数字空格
    public const string OpHoleHex = "ff";     // 符号空格
 
    public static bool TryEncode(string symbol, out string hex) =>
        EncodeMap.TryGetValue(symbol, out hex!);
 
    public static bool TryDecode(string hex, out string symbol) =>
        DecodeMap.TryGetValue(hex.ToLowerInvariant(), out symbol!);
 
    public static bool IsOperatorToken(string token) =>
        EncodeMap.ContainsKey(token);
}