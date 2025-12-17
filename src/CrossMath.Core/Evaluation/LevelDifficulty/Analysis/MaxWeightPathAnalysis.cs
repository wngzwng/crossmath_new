namespace CrossMath.Core.Evaluation.LevelDifficulty.Analysis;

public sealed class MaxWeightPathAnalysis
{
    public int StuckNum { get; init; }
    public IReadOnlyList<int> StuckPoints { get; init; }
    public int? FirstStuckPoint { get; init; }
    public double? FirstStuckPointPercent { get; init; }

    public IReadOnlyList<(int Row, int Col, int LocalDifficulty)> PathCoordinates { get; init; }
}
