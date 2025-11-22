using CrossMath.Core.Exceptions;
using CrossMath.Core.Expressions.Core;

namespace CrossMath.Core.Expressions.Exceptions;

public class ExpressionException : CrossMathException
{
    /// <summary>
    /// 引发异常的表达式（可选）。
    /// 便于调试与结构化日志记录。
    /// </summary>
    public IExpression? Expression { get; }

    /// <summary>
    /// 出错的 Token 索引（如果可以定位的话）。
    /// 如运算符在 index=1 解析失败。
    /// </summary>
    public int? TokenIndex { get; }

    /// <summary>
    /// 对错误的简短说明（可选）。
    /// </summary>
    public string? Reason { get; }

    // ------------------------
    // 构造函数
    // ------------------------

    public ExpressionException(
        string message,
        IExpression? expression = null,
        int? tokenIndex = null,
        string? reason = null,
        Exception? inner = null)
        : base(message, inner)
    {
        Expression = expression;
        TokenIndex = tokenIndex;
        Reason = reason;
    }

    public ExpressionException(string message) : base(message) { }

    public ExpressionException(string message, Exception inner)
        : base(message, inner) { }


    // ------------------------
    // 结构化日志扩展
    // ------------------------
    public override IEnumerable<KeyValuePair<string, object?>> GetLoggingProperties()
    {
        // 1) 基类基础字段
        foreach (var kv in base.GetLoggingProperties())
            yield return kv;

        // 2) 额外字段
        if (Reason != null)
            yield return new("Reason", Reason);

        if (TokenIndex != null)
            yield return new("TokenIndex", TokenIndex);

        if (Expression != null)
        {
            yield return new("ExpressionString", Expression.ToString());
            yield return new("ExpressionTokens", string.Join(" | ", Expression.GetTokens()));
        }
    }
}
