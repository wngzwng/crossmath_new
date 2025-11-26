using CrossMath.Core.Generators.Canvas;

namespace CrossMath.Core.Generators.CompletionCheckers;
// 默认收集和Canvas面板大小一致的布局盘面
public class CompletionChecker : ICompletionChecker
{
    public bool IsComplete(ICanvas canvas)
    {
        return canvas.GetBoundingBoxSize() == canvas.CanvasSize;
    }
}