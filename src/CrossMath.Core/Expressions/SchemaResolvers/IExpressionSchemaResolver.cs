using CrossMath.Core.Expressions.Schema;

namespace CrossMath.Core.Expressions.SchemaResolvers;

public interface IExpressionSchemaResolver
{
    /// <summary>
    /// 根据一组 layout tokens，解析出可解释它的 schema
    /// 若不存在，返回 null 或抛异常（由策略决定）
    /// </summary>
    IExpressionSchema? Resolve(
        IReadOnlyList<string> layoutTokens);
}
