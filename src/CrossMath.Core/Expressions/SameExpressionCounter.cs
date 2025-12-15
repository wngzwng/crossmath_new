    using CrossMath.Core.Expressions.Core;
    using CrossMath.Core.Expressions.Layout;
    using CrossMath.Core.Models;

    namespace CrossMath.Core.Expressions;

    public static class SameExpressionCounter
    {
        public static Dictionary<IExpression, int> StrictCounts(BoardData board)
        {
            var counts = new Dictionary<IExpression, int>();
            foreach (var layout in ExpressionLayoutBuilder.ExtractLayouts(board.Layout, [5, 7]))
            {
                var expr = layout.ToExpression(board);
                counts[expr] = counts.ContainsKey(expr) ? counts[expr] + 1 : 1;
            }
            return counts;
        }
        
        public static Dictionary<string, int> WeakCounts(BoardData board)
        {
            var counts = new Dictionary<string, int>();
            foreach (var layout in ExpressionLayoutBuilder.ExtractLayouts(board.Layout, [5, 7]))
            {
                var expr = layout.ToExpression(board);
                if (!expr.IsFullyFilled) continue;
                
                var weakId = WeakExpressionUtils.GetWeakId(expr);
                counts[weakId] = counts.ContainsKey(weakId) ? counts[weakId] + 1 : 1;
            }
            return counts;
        }
    }