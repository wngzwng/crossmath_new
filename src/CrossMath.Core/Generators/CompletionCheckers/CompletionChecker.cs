
using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Generators.Collectors;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Generators.CompletionCheckers;
// 默认收集和Canvas面板大小一致的布局盘面
public class CompletionChecker : ICompletionChecker
{
    
    private List<Func<ICanvas, bool>> filters = new List<Func<ICanvas, bool>>();
    public bool IsComplete(ICanvas canvas)
    {
        return filters.All(filter => filter(canvas));
    }


    public CompletionChecker AddSigmaFilter(Func<double, bool> sigmaLimiter)
    {
        filters.Add(canvas =>
        {
            var boardLayout = canvas.ExportBoardLayout(needCrop: true);
            var sigma = boardLayout.Sigma();
            return sigmaLimiter(sigma);
        });
        return this;
    }
    
    public CompletionChecker AddSizeFilter(Func<Size, bool> sizeLimiter)
    {
        filters.Add(canvas =>
        {
            var (minPos, maxPos) = canvas.GetMinMaxPosition();
            return sizeLimiter(Size.GetBoundingBoxSize(minPos, maxPos));
        });
        return this;
    }
    
    public CompletionChecker AddFormulaCountFilter(Func<int, bool> formulaCountLimiter)
    {
        filters.Add(canvas => formulaCountLimiter(canvas.CountEquations()));
        return this;
    }


    public CompletionChecker AddBuckerFilter<TK>(BucketCounter<TK> counter)
    {
        filters.Add(canvas => counter.TryIncrement(canvas.ExportBoardLayout()));
        return this;
    }
    

    public CompletionChecker AddCustomFilter(Func<ICanvas, bool> customFilter)
    {
        filters.Add(customFilter);
        return this;
    }
    
    
    
    public void Reset()
    {
        filters.Clear();
    }
}