using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Models;

namespace CrossMath.Core.Generators;

public class LayoutGeneratorSimple
{
    public IEnumerable<BoardLayout> Generate(
        ICanvas initialCanvas,
        LayoutContext context,
        int maxResults = int.MaxValue)
    {
        ArgumentNullException.ThrowIfNull(initialCanvas);
        ArgumentNullException.ThrowIfNull(context);

        var count = 0;
        foreach (var layout in context.SearchPolicy.Search(context, initialCanvas))
        {
            yield return layout;
            if (++count >= maxResults) yield break;
        }
    }
}