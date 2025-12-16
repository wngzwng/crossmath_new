using CrossMath.Core.Types;

namespace CrossMath.Core.Evaluation.LocalDifficulty;

public class LocalEvaluationResult
{
    private Dictionary<RowCol, List<int>> _difficultyRecords;
    
    public IReadOnlyDictionary<RowCol, IReadOnlyList<int>> OriginDifficultyRecords =>
        _difficultyRecords.ToDictionary(
            kv => kv.Key,
            kv => (IReadOnlyList<int>)kv.Value
        );
    
    public IReadOnlyDictionary<RowCol, int> MinDifficultyPerCell => GetMinDifficultyPerCell();

    public LocalEvaluationResult()
    {
        _difficultyRecords = new Dictionary<RowCol, List<int>>();
    }

    public IReadOnlyDictionary<RowCol, int> GetMinDifficultyPerCell()
    {
        return _difficultyRecords.ToDictionary(
            kv => kv.Key,
            kv => kv.Value.Min()
        );
    }

    public void MarkDifficulty(RowCol rowCol, int difficulty)
    {
        // Invariant: each RowCol entry has at least one difficulty mark
        if (!_difficultyRecords.ContainsKey(rowCol))
        {
            _difficultyRecords.Add(rowCol, new List<int>());
        }
        _difficultyRecords[rowCol].Add(difficulty);
    }


    public void Reset()
    {
        _difficultyRecords.Clear();
    }
}