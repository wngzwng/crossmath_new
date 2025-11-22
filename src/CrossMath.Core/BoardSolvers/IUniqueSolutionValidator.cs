using CrossMath.Core.Models;

namespace CrossMath.Core.BoardSolvers;

public interface IUniqueSolutionValidator
{
    bool HasUniqueSolution(BoardData board);
}