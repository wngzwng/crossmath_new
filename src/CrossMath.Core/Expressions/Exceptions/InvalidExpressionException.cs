using CrossMath.Core.Exceptions;
using CrossMath.Core.Expressions.Core;
namespace CrossMath.Core.Expressions.Exceptions;

public class InvalidExpressionException : CrossMathException
{
    /// <summary>错误原因（可选说明）</summary>
    public string? Reason { get; }

    /// <summary>引发异常的表达式（可选）</summary>
    public IExpression? Expression { get; }

    /// <summary>错误的 Token 索引（若有）</summary>
    public int? TokenIndex { get; }

    public InvalidExpressionException(
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

    /// <summary>
    /// 结构化日志使用（Serilog / ApplicationInsights / OpenTelemetry）
    /// </summary>
    public override IEnumerable<KeyValuePair<string, object?>> GetLoggingProperties()
    {
        // 继承父类基础字段
        foreach (var kv in base.GetLoggingProperties())
            yield return kv;

        if (Reason != null)
            yield return new("Reason", Reason);

        if (Expression != null)
        {
            yield return new("ExpressionString", Expression.ToString());
            yield return new("ExpressionTokens", string.Join(" | ", Expression.GetTokens()));
        }

        if (TokenIndex.HasValue)
            yield return new("TokenIndex", TokenIndex.Value);
    }
}
