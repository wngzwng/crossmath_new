using CrossMath.Core.Types;

namespace CrossMath.Core.Analytics.EmptyBoard;

public class FormulaCapacityRegistry
{
    private readonly Dictionary<(int Height, int Width), int> _map = new();

    public FormulaCapacityRegistry Register(int height, int width, int maxFormula)
    {
        _map[(height, width)] = maxFormula;
        return this; // 链式写法
    }

    public bool TryGetMaxFormula(int height, int width, out int value)
    {
        return _map.TryGetValue((height, width), out value);
    }
    
    public bool TryGetMaxFormula(Size size, out int value)
    {
        return _map.TryGetValue((size.Height, size.Width), out value);
    }

    public static FormulaCapacityRegistry CreateDefaultRegistry()
    {
        /*
              (7, 5):   7,
              (7, 7):   8,
              (9, 7):   9,
              (7, 9):   9,
              (9, 9):   10,
              (7, 11):  13,
              (11, 7):  13,
              (11, 9):  15,
              (9, 11):  15,
              (11, 11): 21,
              (13, 11): 24,
              (13, 13): 26,
         */
        return new FormulaCapacityRegistry()
            .Register(7, 5, 7)
            .Register(7, 7, 8)
            .Register(9, 7, 9)
            .Register(7, 9, 9)
            .Register(9, 9, 10)
            .Register(7, 11, 13)
            .Register(11, 7, 13)
            .Register(11, 9, 15)
            .Register(9, 11, 15)
            .Register(11, 11, 21)
            .Register(13, 11, 24)
            .Register(13, 13, 26);
    }
}
