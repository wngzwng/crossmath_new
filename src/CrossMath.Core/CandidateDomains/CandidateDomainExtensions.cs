using CrossMath.Core.Expressions.Core;
using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Types;

namespace CrossMath.Core.CandidateDomains;

public static class CandidateDomainExtensions
{
    public static CandidateTable<RowCol, string> BuildCandidateTable(
        this ExpressionLayout layout, IExpression origin, IEnumerable<IExpression> solved)
    {
        var t = new CandidateTable<RowCol, string>();
        foreach (var s in solved) t.AddRow(ExtractCandidateRow(layout, origin, s));
        return t;
    }

    public static CandidateRow<RowCol, string> ExtractCandidateRow(
       this ExpressionLayout layout, IExpression origin, IExpression solved)
    {
        var map = new Dictionary<RowCol, string>();
        for (int i = 0; i < layout.Length; i++)
            if (origin[i] != solved[i])
                map[layout[i]] = solved[i];
        return new CandidateRow<RowCol, string>(map);
    }
}