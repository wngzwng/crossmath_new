using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Generators.ExpandControllers;
using CrossMath.Core.Types;

namespace business.works.Layout;

public class FormulaCountExpandController : IExpandController
{
    public IReadOnlyDictionary<Size, int> SizeToMaxFormulaCountMap { get; }

    public int MostExp7Count = 2;

    public FormulaCountExpandController(Dictionary<Size, int> map)
    {
        SizeToMaxFormulaCountMap = map;
    }
    

    /// <summary>
    /// 是否允许继续扩展（递归深入）
    /// 条件：
    /// 1. Exp7 数量不超过全局上限（默认2个）
    /// 2. 当前尺寸下公式总数未超标
    /// </summary>
    public bool ShouldExpand(ICanvas canvas, Placement? lastPlacement, int depth)
    {
        // 条件1：Exp7 不能太多（防止放太多超大件导致无法收敛）
        if (!HasAtMostExp7(canvas, MostExp7Count))
            return false;

        // 条件2：当前尺寸有配额限制的话，不能超
        if (SizeToMaxFormulaCountMap.TryGetValue(canvas.CanvasSize, out int maxAllowed))
        {
            return canvas.CountEquations() < maxAllowed; // 注意：这里建议用 < 而不是 <=
            // 因为放置后才会+1，这里是“放置前”判断
        }

        // 没配额限制 = 无限制
        return true;
    }
    public bool HasAtMostExp7(ICanvas canvas, int maxExp7Count)
    {
        var expr7 = canvas.CountSevens();
        return expr7 <= maxExp7Count;
    }
}
