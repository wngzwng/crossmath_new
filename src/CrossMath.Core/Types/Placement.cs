namespace CrossMath.Core.Types;


public readonly record struct Placement(int Row, int Col, Direction Direction, int Length)
{


    public override string ToString() =>
        $"{(Direction == Direction.Horizontal ? 'H' : 'V')}{Length} @ ({Row},{Col})";
}