using CrossMath.Core.Models;

namespace CrossMath.Core.Expressions.Layout;

public static class ExpressionLayoutBuilder
{
        public static List<ExpressionLayout> ExtractLayouts(
            BoardLayout layout,
            IEnumerable<int> allowExpressionLengths)
        {
            return ExpressionLayoutBuilderCore.ExtractLayouts(
                layout.Height,
                layout.Width,
                layout.IsValid,
                allowExpressionLengths);
        }
}
