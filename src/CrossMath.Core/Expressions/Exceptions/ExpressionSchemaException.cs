namespace CrossMath.Core.Expressions.Exceptions;

public class ExpressionSchemaException : ExpressionException
{
    /// <summary>
    /// 表达式实际的 token 数量。
    /// </summary>
    public int TokenCount { get; }

    /// <summary>
    /// 支持的表达式长度（如 5 和 7）。
    /// </summary>
    public IReadOnlyList<int>? AllowedLengths { get; }

    public ExpressionSchemaException(
        string message,
        int tokenCount,
        IReadOnlyList<int>? allowedLengths = null,
        string? reason = null,
        Exception? inner = null)
        : base(message, null, null, reason, inner)
    {
        TokenCount = tokenCount;
        AllowedLengths = allowedLengths;
    }

    public override IEnumerable<KeyValuePair<string, object?>> GetLoggingProperties()
    {
        // 1) 基类内容
        foreach (var kv in base.GetLoggingProperties())
            yield return kv;

        // 2) Schema 相关字段
        yield return new("TokenCount", TokenCount);

        if (AllowedLengths != null)
            yield return new("AllowedLengths", string.Join(",", AllowedLengths));

        if (Reason != null)
            yield return new("Reason", Reason);
    }
}
