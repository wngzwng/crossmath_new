using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Types;

namespace CrossMath.Core.Generators.PlacementOrderingPolicies;

public class RandomPlacementOrderingPolicy: IPlacementOrderingPolicy
{
    private readonly Random _rng = new Random();
    public IEnumerable<Placement> Order(IEnumerable<Placement> placements, ICanvas currentCanvas, Random? random = null)
    {
        random = random ?? _rng;
        var array = placements.ToArray();
        // if (array.Length == 0) return default; // 会有bug
        if (array.Length == 0) return Enumerable.Empty<Placement>();

        // Fisher-Yates 现代洗牌
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
        return array;
    }
}