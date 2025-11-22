using CrossMath.Core.Expressions.Exceptions;
using CrossMath.Core.Expressions.Core;

namespace CrossMath.Core.Expressions.Schema;

public static class ExpressionSchemaFactory
{
    private static readonly Dictionary<int, IExpressionSchema> _cache = new Dictionary<int, IExpressionSchema>
    {
        [5] = new Expression5Schema(),
        [7] = new Expression7Schema()
    };

    /// <summary>
    /// 根据 token 数量创建或返回缓存的 Schema。
    /// </summary>
    public static IExpressionSchema Create(int tokenCount)
    {
        if (_cache.TryGetValue(tokenCount, out var schema))
            return schema;

        throw new ExpressionSchemaException(
            message: $"不支持的表达式 Token 数量: {tokenCount}（仅支持 5 或 7）",
            tokenCount: tokenCount,
            allowedLengths: new[] { 5, 7 },
            reason: "Invalid expression schema"
        );

    }

    /// <summary>
    /// 根据表达式实例推断 Schema。
    /// </summary>
    public static IExpressionSchema FromExpression(IExpression expression)
    {
        if (expression is null)
            throw new ExpressionSchemaException("Expression 不能为空。", 0);

        return Create(expression.Length);
    }
}