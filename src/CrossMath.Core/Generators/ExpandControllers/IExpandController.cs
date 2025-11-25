using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Types;

namespace CrossMath.Core.Generators.ExpandControllers;

public interface IExpandController
{
    bool ShouldExpand(ICanvas canvas, Placement? lastPlacement, int depth);
}