namespace CrossMath.Core.Evaluation.LevelDifficulty.Analysis;

public sealed class MaxWeightPathAnalysis
{
    public int StuckNum { get; init; }
    public IReadOnlyList<int> StuckPoints { get; init; }
    public int? FirstStuckPoint { get; init; }
    public double? FirstStuckPointPercent { get; init; }

    public IReadOnlyList<(int Row, int Col, int LocalDifficulty)> PathCoordinates { get; init; }


    public string ToDebugString()
    {
        var coords = PathCoordinates
            // .Take(maxPath)
            .Select(c => $"({c.Row},{c.Col})[L{c.LocalDifficulty}]");

        return
            $"StuckNum={StuckNum}, " +
            $"FirstStuck={FirstStuckPoint}@{FirstStuckPointPercent:P1}, " +
            $"StuckPoints=[{string.Join(", ", StuckPoints)}], " +
            $"PathPreview={string.Join(" → ", coords)}" + "";
        // (PathCoordinates.Count > maxPath ? " → ..." : "");
    }
}
