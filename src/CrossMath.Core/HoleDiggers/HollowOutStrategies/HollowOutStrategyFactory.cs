namespace CrossMath.Core.HoleDiggers.HollowOutStrategies;

public class HollowOutStrategyFactory
{
    public static IHollowOutStrategy CreateStrategy(HollowOutStrategyType strategyType, bool allowHollowOutOperator = false)
    {
        return strategyType switch
        {
            HollowOutStrategyType.IntersectionPriority => new IntersectionPriorityStrategy(allowHollowOutOperator),
            HollowOutStrategyType.NonFocusPriority => new NonFocusPriorityStrategy(allowHollowOutOperator),
            HollowOutStrategyType.Random => new RandomStrategy(allowHollowOutOperator),
            HollowOutStrategyType.FrequentNumberPriority => new FrequentNumberPriorityStrategy(),
            HollowOutStrategyType.NumberFriendlinessPriority => new NumberFriendlinessPriorityStrategy(),
            HollowOutStrategyType.LargeNumberPriority => new LargeNumberPriorityStrategy(),
            HollowOutStrategyType.SmallNumberPriority => new SmallNumberPriorityStrategy(),
            _ => new RandomStrategy()
        };
    }
}