using CrossMath.Core.HoleDiggers;
using CrossMath.Core.HoleDiggers.HoleCount;
using CrossMath.Core.HoleDiggers.HoleVadidators;
using CrossMath.Core.HoleDiggers.HollowOutStrategies;
using CrossMath.Core.Models;

namespace business.works.Hole;

public class HoleRunner
{
    // 一个终盘，
    private readonly HoleDigger _holeDigger = new HoleDigger();
    private const int MaxRetry = 3;

    public IEnumerable<BoardData> IterHollowOutBoard(BoardData boardData)
    {
        foreach (var hollowOutStrategyType in IterHollowOutStrategyType())
        {
            var strategy = HollowOutStrategyFactory.CreateStrategy(hollowOutStrategyType);
            
            for (int attempt = 0; attempt < MaxRetry; attempt++)
            {
                var context = CreateContext(boardData, strategy);

                if (_holeDigger.TryHollowOut2(context, out var resultBoard))
                {
                    yield return resultBoard;
                    break;
                }
            }
        }
    }
    
    private HollowOutContext CreateContext(BoardData board, IHollowOutStrategy strategy)
    {
        var holeCountType = RandomHoleCountTypeSelector.GetRandomByDefaultWeight();
        return HollowOutContext.Create(
            board.Clone(),
            holeCountType,
            strategy,
            DefaultHoleValidator.Create()
            );
    }
    
    private IEnumerable<HollowOutStrategyType> IterHollowOutStrategyType()
    {
        foreach (var strategy in Enum.GetValues<HollowOutStrategyType>())
        {
            yield return strategy;
        }
    }
}