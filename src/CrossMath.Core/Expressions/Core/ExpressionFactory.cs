using CrossMath.Core.Expressions.Exceptions;

namespace CrossMath.Core.Expressions.Core;

public static class ExpressionFactory
{
    // 只需在这里注册新类型，永远不用改 switch！
    private static readonly Dictionary<int, Func<IExpression>> Creators = new()
    {
        [5] = () => new Expression5(),
        [7] = () => new Expression7(),
        // 以后加：
        // [9] = () => new Expression9(),
        // [11] = () => new Expression11(),
    };

    private static IExpression CreateByLength(int length)
    {
        if (Creators.TryGetValue(length, out var creator))
            return creator();

        throw new InvalidExpressionException($"不支持的表达式长度：{length}（当前支持：{string.Join(", ", Creators.Keys.OrderBy(k => k))})");
    }

    // 公开方法统一走这个核心创建逻辑
    public static IExpression CreateEmpty(int length) => CreateByLength(length);

    public static IExpression FromTokens(IReadOnlyList<string> tokens)
    {
        if (tokens is null) throw new ArgumentNullException(nameof(tokens));
        if (tokens.Count == 0) throw new ArgumentException("tokens 不能为空", nameof(tokens));

        var expr = CreateByLength(tokens.Count);

        for (int i = 0; i < tokens.Count; i++)
            expr[i] = tokens[i];

        return expr;
    }

    public static IExpression FromArray(string[] arr)
        => FromTokens(arr ?? throw new ArgumentNullException(nameof(arr)));
}