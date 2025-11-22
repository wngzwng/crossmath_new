namespace CrossMath.Core.Exceptions;

public static class ExceptionLogExtensions
{
    public static IReadOnlyDictionary<string, object?> ToLogDictionary(this CrossMathException ex)
    {
        return ex.GetLoggingProperties().ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}

/*
// 使用方式
catch (CrossMathException ex)
{
    _logger.LogError(ex, "CrossMath error {@Props}", ex.ToLogDictionary());
}
*/
