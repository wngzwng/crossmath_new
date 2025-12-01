using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.HoleDiggers.HoleVadidators;

public interface IHoleValidator
{
    bool IsValidHollowOut(BoardData board, RowCol coord);
}