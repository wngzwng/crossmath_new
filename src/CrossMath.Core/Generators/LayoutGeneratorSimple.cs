using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Models;

namespace CrossMath.Core.Generators;

public class LayoutGeneratorSimple
{
    public IEnumerable<BoardLayout> Generate(
        ICanvas initialCanvas,
        LayoutGenContext context)
    {
        ArgumentNullException.ThrowIfNull(initialCanvas);
        ArgumentNullException.ThrowIfNull(context);

        var count = 0;
        foreach (var layout in context.Go.Search(context, initialCanvas))
        {
            yield return layout;
            count++;
            
            if (context.StopPolicy.ShouldStop(count, context, layout))
            {
                Console.WriteLine($"Stop: {initialCanvas.CanvasSize}, count: {count}");
                yield break;
            }
        }
    }
}