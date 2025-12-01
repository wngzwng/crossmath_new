using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Generators.ExpandControllers;
using CrossMath.Core.Types;

namespace business.works.Layout;

public class FormulaCountExpandController : IExpandController
{
    public IReadOnlyDictionary<Size, int> SizeToMaxFormulaCountMap { get; }

    public FormulaCountExpandController(Dictionary<Size, int> map)
    {
        SizeToMaxFormulaCountMap = map;
    }

    public bool ShouldExpand(ICanvas canvas, Placement? lastPlacement, int depth)
    {
        var count = canvas.CountEquations();
        return !SizeToMaxFormulaCountMap.TryGetValue(canvas.CanvasSize, out var max)
               || count < max;
    }
}
