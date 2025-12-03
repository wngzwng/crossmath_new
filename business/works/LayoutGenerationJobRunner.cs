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
        var ctx = job.CreateContext(_globalSeen);

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
                foreach (var layout in _generator.Generate(initCanvas, ctx))
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
}
