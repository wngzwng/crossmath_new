using CrossMath.Core.CandidateDomains;
using CrossMath.Core.Types;

namespace CrossMath.Core.CSP;

public record CSPResult(
    Dictionary<RowCol, HashSet<string>> VariableDomains,
    CandidateDomainManager<RowCol, string> ExpressionCandidates);