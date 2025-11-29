using CrossMath.Core.Types;
namespace CrossMath.Core.Models;

public class BoardSolution
{
    public BoardData data { get; }
    
    public IReadOnlyDictionary<RowCol, string> solutionMap { get;}

    public BoardSolution(BoardData data, IReadOnlyDictionary<RowCol, string> solutionMap)
    {
        this.data = data;
        this.solutionMap = solutionMap;
    }
}