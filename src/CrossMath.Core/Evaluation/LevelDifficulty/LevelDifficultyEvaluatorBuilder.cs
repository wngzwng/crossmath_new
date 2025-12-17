using CrossMath.Core.Evaluation.LevelDifficulty.EvaluationTraces;
using CrossMath.Core.Evaluation.LevelDifficulty.ScoreStrategies;
using CrossMath.Core.Evaluation.LevelDifficulty.SelectionPolicies;
using CrossMath.Core.Evaluation.LevelDifficulty.WeightCalculators;
using CrossMath.Core.Evaluation.LocalDifficulty;

namespace CrossMath.Core.Evaluation.LevelDifficulty;

public sealed class LevelDifficultyEvaluatorBuilder
{
    private LocalDifficultyEvaluator? _localEvaluator;
    private IScoreStrategy? _scoreStrategy;
    private IWeightCalculator? _weightCalculator;
    private ISelectionPolicy? _selectionPolicy;
    private IEvaluationTrace? _trace;

    internal LevelDifficultyEvaluatorBuilder() { }

    public LevelDifficultyEvaluatorBuilder WithLocalEvaluator(
        LocalDifficultyEvaluator evaluator)
    {
        _localEvaluator = evaluator;
        return this;
    }

    public LevelDifficultyEvaluatorBuilder WithScoreStrategy(
        IScoreStrategy strategy)
    {
        _scoreStrategy = strategy;
        return this;
    }

    public LevelDifficultyEvaluatorBuilder WithWeightCalculator(
        IWeightCalculator calculator)
    {
        _weightCalculator = calculator;
        return this;
    }

    public LevelDifficultyEvaluatorBuilder WithSelectionPolicy(
        ISelectionPolicy policy)
    {
        _selectionPolicy = policy;
        return this;
    }

    public LevelDifficultyEvaluatorBuilder WithTrace(
        IEvaluationTrace trace)
    {
        _trace = trace;
        return this;
    }

    public LevelDifficultyEvaluator Build()
    {
        return new LevelDifficultyEvaluator(
            localEvaluator: _localEvaluator
                            ?? throw new InvalidOperationException("LocalEvaluator not set"),
            scoreStrategy: _scoreStrategy
                           ?? throw new InvalidOperationException("ScoreStrategy not set"),
            weightCalculator: _weightCalculator
                              ?? throw new InvalidOperationException("WeightCalculator not set"),
            selectionPolicy: _selectionPolicy
                             ?? throw new InvalidOperationException("SelectionPolicy not set"),
            trace: _trace
        );
    }
}
