using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Analytics.EmptyBoard;
/// <summary>
/// 特殊算式分析器
/// ------------------------------------------------
/// 用于统计两类特殊算式：
/// 1. L形算式：仅有一个交点，且交点在头/尾
/// 2. Z形算式：有两个交点，且均在头/尾，且与其相交的算式方向相反
/// </summary>
public class SpecialExpressionAnalyzer
{
    private readonly List<ExpressionLayout> _expressionLayouts;
    private readonly Dictionary<RowCol, HashSet<string>> _posToExpr;
    private readonly Dictionary<string, HashSet<RowCol>> _exprToPos;

    public SpecialExpressionAnalyzer(List<ExpressionLayout> expressionLayouts)
    {
        _expressionLayouts = expressionLayouts ?? throw new ArgumentNullException(nameof(expressionLayouts));
        _posToExpr = ExpressionLayoutGraphUtils.BuildPosToExprMap(expressionLayouts);
        _exprToPos = ExpressionLayoutGraphUtils.BuildExprToPosMap(expressionLayouts);
    }

    // ------------------------------------------------------------
    // 构建器接口
    // ------------------------------------------------------------
    public static SpecialExpressionAnalyzer FromLayout(BoardLayout layout, List<int> allowedLengths)
    {
        var exprLayouts = ExpressionLayoutBuilder.ExtractLayouts(layout, allowedLengths);
        return new SpecialExpressionAnalyzer(exprLayouts);
    }

    // ------------------------------------------------------------
    // 获取交点
    // ------------------------------------------------------------
    public List<RowCol> GetCrossPoints(ExpressionLayout expr)
    {
        var crossPoints = new List<RowCol>();
        foreach (var pos in expr.Cells)
        {
            if (_posToExpr.TryGetValue(pos, out var linkedIds) && linkedIds.Count > 1)
            {
                crossPoints.Add(pos);
            }
        }
        return crossPoints;
    }

    public List<ExpressionLayout> GetRelatedExpressions(RowCol pos, string currentExprId)
    {
        if (!_posToExpr.TryGetValue(pos, out var relatedIds))
            return new List<ExpressionLayout>();

        var filteredIds = relatedIds.Where(id => id != currentExprId).ToHashSet();
        return _expressionLayouts.Where(expr => filteredIds.Contains(expr.Id)).ToList();
    }

    // ------------------------------------------------------------
    // 特殊算式判定
    // ------------------------------------------------------------
    public bool IsLShape(ExpressionLayout expr)
    {
        var crossPoints = GetCrossPoints(expr);
        if (crossPoints.Count != 1)
            return false;

        var crossPos = crossPoints[0];
        // 判断交点是否为头或尾
        if (crossPos != expr.Cells[0] && crossPos != expr.Cells[^1])
            return false;

        // 对应的相交表达式也要是端点交叉
        var relatedExprs = GetRelatedExpressions(crossPos, expr.Id.Value);
        if (relatedExprs.Count == 0)
            return false;

        var relatedExpr = relatedExprs[0];
        if (crossPos != relatedExpr.Cells[0] && crossPos != relatedExpr.Cells[^1])
            return false;

        return true;
    }

    public bool IsZShape(ExpressionLayout expr)
    {
        var crossPoints = GetCrossPoints(expr);
        if (crossPoints.Count != 2)
            return false;

        // 两个交点是否在头尾
        if (!crossPoints.All(pos => pos == expr.Cells[0] || pos == expr.Cells[^1]))
            return false;

        // 记录相交算式与交点位置
        var relatedExprPairs = new List<(ExpressionLayout Expr, RowCol Pos)>();
        foreach (var pos in crossPoints)
        {
            var relatedExprs = GetRelatedExpressions(pos, expr.Id.Value);
            if (relatedExprs.Count != 1)
                return false;

            var relatedExpr = relatedExprs[0];
            // 相交算式交点也必须在其头尾
            if (pos != relatedExpr.Cells[0] && pos != relatedExpr.Cells[^1])
                return false;

            relatedExprPairs.Add((relatedExpr, pos));
        }

        // 两条相交算式方向应相同
        if (relatedExprPairs[0].Expr.Direction != relatedExprPairs[1].Expr.Direction)
            return false;

        // 且与当前算式方向相反
        if (relatedExprPairs[0].Expr.Direction == expr.Direction)
            return false;

        // 检查头尾类型
        string GetHeadOrTail(ExpressionLayout expression, RowCol position)
        {
            if (position == expression.Cells[0]) return "HEAD";
            if (position == expression.Cells[^1]) return "TAIL";
            return "MIDDLE";
        }

        var headTail1 = GetHeadOrTail(relatedExprPairs[0].Expr, relatedExprPairs[0].Pos);
        var headTail2 = GetHeadOrTail(relatedExprPairs[1].Expr, relatedExprPairs[1].Pos);

        // 若两个相交算式交点都是同头或同尾，则为"门形"，排除
        if (headTail1 == headTail2)
            return false;

        return true;
    }

    // ------------------------------------------------------------
    // 统计接口
    // ------------------------------------------------------------
    public int CountLShape()
    {
        return _expressionLayouts.Count(IsLShape);
    }

    public int CountZShape()
    {
        return _expressionLayouts.Count(IsZShape);
    }

    public Dictionary<string, int> Summarize()
    {
        return new Dictionary<string, int>
        {
            ["L_shape_count"] = CountLShape(),
            ["Z_shape_count"] = CountZShape()
        };
    }
}