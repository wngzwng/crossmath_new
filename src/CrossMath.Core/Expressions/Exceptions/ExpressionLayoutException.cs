
using CrossMath.Core.Types;
using CrossMath.Core.Exceptions;
using CrossMath.Core.Models;
using CrossMath.Core.Expressions.Layout;
namespace CrossMath.Core.Expressions.Exceptions;
public class ExpressionLayoutException : CrossMathException
{
    /// <summary>
    /// 引发错误的盘面布局（可选）
    /// </summary>
    public BoardLayout? Layout { get; }

    /// <summary>
    /// 引发错误的表达式布局（可选）
    /// </summary>
    public ExpressionLayout? ExpressionLayout { get; }

    /// <summary>
    /// 错误原因（可选文本）
    /// </summary>
    public string? Reason { get; }

    /// <summary>
    /// 行号（可选）
    /// </summary>
    public int? Row { get; }

    /// <summary>
    /// 列号（可选）
    /// </summary>
    public int? Col { get; }

    /// <summary>
    /// 出错的表达式索引（可选）
    /// </summary>
    public int? ExpressionIndex { get; }

    public ExpressionLayoutException(
        string message,
        BoardLayout? layout = null,
        ExpressionLayout? expressionLayout = null,
        int? row = null,
        int? col = null,
        int? expressionIndex = null,
        string? reason = null,
        Exception? inner = null)
        : base(message, inner)
    {
        Layout = layout;
        ExpressionLayout = expressionLayout;
        Row = row;
        Col = col;
        ExpressionIndex = expressionIndex;
        Reason = reason;
    }

    /// <summary>
    /// 结构化日志专用
    /// </summary>
    public override IEnumerable<KeyValuePair<string, object?>> GetLoggingProperties()
    {
        foreach (var kv in base.GetLoggingProperties())
            yield return kv;

        if (Reason != null)
            yield return new("Reason", Reason);

        if (Row.HasValue)
            yield return new("Row", Row.Value);

        if (Col.HasValue)
            yield return new("Col", Col.Value);

        if (ExpressionIndex.HasValue)
            yield return new("ExpressionIndex", ExpressionIndex.Value);

        if (Layout != null)
        {
            yield return new("BoardWidth", Layout.Value.Width);
            yield return new("BoardHeight", Layout.Value.Height);
            yield return new("RawLayout", Layout.Value.LayoutStr);
        }

        if (ExpressionLayout != null)
        {
            // yield return new("ExprStartRow", ExpressionLayout.Start.Row);
            // yield return new("ExprStartCol", ExpressionLayout.Start.Col);
            yield return new("ExprDirection", ExpressionLayout.Direction.ToString());
            yield return new("ExprLength", ExpressionLayout.Length);
        }
    }
}
