namespace CrossMath.Core.Exceptions;

[Serializable]
public abstract class CrossMathException : Exception
{
    protected CrossMathException(string message)
        : base(message) { }

    protected CrossMathException(string message, Exception? inner)
        : base(message, inner) { }

    /// <summary>
    /// 用于结构化日志（Serilog / ApplicationInsights / OpenTelemetry）
    /// </summary>
    public virtual IEnumerable<KeyValuePair<string, object?>> GetLoggingProperties()
    {
        yield return new KeyValuePair<string, object?>("ExceptionType", GetType().Name);
        yield return new KeyValuePair<string, object?>("Message", Message);

        if (InnerException != null)
            yield return new KeyValuePair<string, object?>("InnerException", InnerException.GetType().Name);
    }
}
