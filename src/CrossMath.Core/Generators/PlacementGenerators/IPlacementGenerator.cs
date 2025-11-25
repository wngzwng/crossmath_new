using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Types;

namespace CrossMath.Core.Generators.PlacementGenerators;

public interface IPlacementGenerator
{
    IEnumerable<Placement> Generate(ICanvas canvas, int expressionLength, CrossType crossType);
}