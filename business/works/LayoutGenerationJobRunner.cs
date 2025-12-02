using business.works.Layout;
using CrossMath.Core.Generators;
using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Models;
using CrossMath.Core.Types;
namespace business.works;

public sealed class LayoutGenerationJobRunner
{
    private readonly LayoutGeneratorSimple _generator = new();
    private readonly HashSet<ulong> _globalSeen = new();

    public IEnumerable<BoardLayout> Run(LayoutGenerationJob job)
    {
        var ctx = BuildContext(job);

        if (job.InitPlacements.Count() == 0)
        {
            yield break;
        }

        int found = 0;

        var initPlacements = job.InitPlacements;
        foreach (Placement initPlacement in initPlacements)
        {
            Console.WriteLine($"\n 初始点： {initPlacement}");
            var initCanvas = new LayoutCanvas(job.CanvasSize);
            if (initCanvas.TryApplyPlacement(initPlacement, out _))
            {
                foreach (var layout in _generator.Generate(initCanvas, ctx, job.TargetCount))
                {
                    found++;

                    job.ProgressCallback?.Invoke(found, job.TargetCount);

                    yield return layout;

                    if (found >= job.TargetCount)
                        yield break;
                }
            }
        }
    }

    private LayoutGenContext BuildContext(LayoutGenerationJob job)
    {
        return new LayoutGenContext
        {
            PlacementGenerator = job.PlacementGenerator,
            ExpandController = new FormulaCountExpandController(
                new Dictionary<Size, int>
                {
                    [job.CanvasSize] = job.MaxFormulaCount
                }),

            CompletionChecker = new CommonCompletionChecker()
                .AddSizeFilter(size => size.Equals(job.CanvasSize))
                .AddFormulaCountFilter(cnt => job.MinFormulaCount <= cnt && cnt <= job.MaxFormulaCount)
                .AddSigmaFilter(sigma => sigma <= job.MaxSigma)
                .AddCustomFilter(canvas => 
                    canvas.CountEquations(expLayout => expLayout.Length == 7) switch
                    {
                        1 or 2 => true,
                        _ => false
                    }),
            
            GlobalSeen = _globalSeen
        };
    }
}
