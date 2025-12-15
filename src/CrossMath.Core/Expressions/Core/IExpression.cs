namespace CrossMath.Core.Expressions.Core;
public interface IExpression : IEquatable<IExpression>
{
    /// <summary>
    /// 表达式的 Token 数量。
    /// 对于 CrossMath，一般是 5 或 7。
    /// </summary>
    int Length { get; }

    /// <summary>
    /// 读取或设置指定位置的 Token。
    /// Token 可以是数字、运算符、等号或空占位符。
    /// </summary>
    string this[int index] { get; set; }

    /// <summary>
    /// 获取表达式的全部 Token 的快照（只读）。
    /// </summary>
    List<string> GetTokens();

    /// <summary>
    /// 是否所有 Token 都已填满（无空白）。
    /// </summary>
    bool IsFullyFilled { get; }

    /// <summary>
    /// 判断表达式在数学上是否成立。
    /// 如 '1 + 2 = 3' 或 '3 * 2 + 1 = 7'。
    /// </summary>
    bool Evaluate();

    /// <summary>
    /// 深拷贝表达式。
    /// 返回一个完全独立的新实例。
    /// </summary>
    IExpression Clone();
}
