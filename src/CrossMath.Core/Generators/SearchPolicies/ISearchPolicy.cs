using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Models;

namespace CrossMath.Core.Generators.SearchPolicies;

public interface ISearchPolicy
{
    IEnumerable<BoardLayout> Search(LayoutGenContext ctx, ICanvas initialCanvas);
}