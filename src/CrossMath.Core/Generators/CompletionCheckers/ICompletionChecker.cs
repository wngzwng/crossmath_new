using CrossMath.Core.Generators.Canvas;

namespace CrossMath.Core.Generators.CompletionCheckers;

public interface ICompletionChecker
{
    bool IsComplete(ICanvas canvas);
}
