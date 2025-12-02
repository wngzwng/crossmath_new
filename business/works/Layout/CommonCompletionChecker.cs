using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Generators.CompletionCheckers;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace business.works.Layout;

public class CommonCompletionChecker: ICompletionChecker
{
    private List<Func<ICanvas, bool>> filters = new List<Func<ICanvas, bool>>();
    public bool IsComplete(ICanvas canvas)
    {
        return filters.All(filter => filter(canvas));
    }


    public CommonCompletionChecker AddSigmaFilter(Func<double, bool> sigmaLimiter)
    {
        filters.Add(canvas =>
        {
            var boardLayout = canvas.ExportBoardLayout(needCrop: true);
            var sigma = boardLayout.Sigma();
            return sigmaLimiter(sigma);
        });
        return this;
    }
    
    public CommonCompletionChecker AddSizeFilter(Func<Size, bool> sizeLimiter)
    {
        filters.Add(canvas =>
        {
            var (minPos, maxPos) = canvas.GetMinMaxPosition();
            return sizeLimiter(Size.GetBoundingBoxSize(minPos, maxPos));
        });
        return this;
    }
    
    public CommonCompletionChecker AddFormulaCountFilter(Func<int, bool> formulaCountLimiter)
    {
        filters.Add(canvas => formulaCountLimiter(canvas.CountEquations()));
        return this;
    }
    
    

    public CommonCompletionChecker AddCustomFilter(Func<ICanvas, bool> customFilter)
    {
        filters.Add(customFilter);
        return this;
    }


    public void Reset()
    {
        filters.Clear();
    }
}