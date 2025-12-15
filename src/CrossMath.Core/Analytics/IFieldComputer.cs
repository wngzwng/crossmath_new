using CrossMath.Core.Models;

namespace CrossMath.Core.Analytics;

public interface IFieldComputer<T>
{
    T Compute(BoardLayout layout, BoardData? board);
}
