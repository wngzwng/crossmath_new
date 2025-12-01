using CrossMath.Core.Types;

namespace CrossMath.Core.HoleDiggers.HollowOutStrategies;

public class RandomStrategy:IHollowOutStrategy
{
    public RowCol? GetNextHoleCoordinate(HollowOutContext context, IEnumerable<RowCol>? candidatePositions = null)
    {
        throw new NotImplementedException();
    }
}