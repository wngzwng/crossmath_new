namespace CrossMath.Core.HoleDiggers.HollowOutStrategies;

public class HollowOutStrategyFactory
{
    public static IHollowOutStrategy CreateStrategy(HollowOutStrategyType strategyType)
    {
        return strategyType switch
        {
            HollowOutStrategyType.IntersectionPriority => new IntersectionPriorityStrategy(),
            HollowOutStrategyType.NonFocusPriority => new NonFocusPriorityStrategy(),
            HollowOutStrategyType.Random => new RandomStrategy(),
            HollowOutStrategyType.FrequentNumberPriority => new FrequentNumberPriorityStrategy(),
            HollowOutStrategyType.NumberFriendlinessPriority => new NumberFriendlinessPriorityStrategy(),
            HollowOutStrategyType.LargeNumberPriority => new LargeNumberPriorityStrategy(),
            _ => new RandomStrategy()
        };
    }
}