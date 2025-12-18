using CrossMath.Core.Codec;
using CrossMath.Core.Models;
using CrossMath.Service.Hole;
using CrossMath.Service.Jobs;
using CrossMath.Service.Utils;
using Microsoft.Extensions.Logging;

namespace CrossMath.Service.Pipelines;

public sealed class InitBoardResult
{
    public BoardData Board { get; }

    public string StartInfo { get; }
    public IReadOnlyList<string> Answers { get; }
    public int EmptyCellCount { get; }

    public InitBoardResult(BoardData board)
    {
        Board = board;
        Answers = board.PossibleAnswers.ToList();
        EmptyCellCount = board.Holes.Count;

        var (startInfo, _) = BoardDataCodec.Encode(board);
        StartInfo = startInfo;
    }
}


public class HoleBoardPipeline
{
    private readonly HoleRunner _runner;

    public HoleBoardPipeline(HoleRunner runner)
    {
        _runner = runner;
    }

    public IEnumerable<InitBoardResult> Run(BoardData filledBoard)
    {
        foreach (var initBoard in _runner.IterHollowOutBoard(filledBoard))
        {
            yield return new InitBoardResult(initBoard);
        }
    }
}