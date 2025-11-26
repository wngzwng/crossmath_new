using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Types;

namespace CrossMath.Core.Generators.ExpandControllers;

// 默认不会停止展开递归
public class ExpandComtroller : IExpandController
{
    public bool ShouldExpand(ICanvas canvas, Placement? lastPlacement, int depth)
    {
        return true;
    }
}