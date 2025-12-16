namespace CrossMath.Core.Evaluation.LocalDifficulty;

public interface ILocalDifficultyLayer
{
    int Difficulty { get; }

    bool CanEvaluate(LocalDifficultyContext ctx) => true;

    void Evaluate(LocalDifficultyContext ctx);
}
