using CrossMath.Core.Codec;
using CrossMath.Core.Types;
using CrossMath.Core.Evaluation.LevelDifficulty.EvaluationTraces;
using CrossMath.Core.Evaluation.LevelDifficulty.ScoreStrategies;
using CrossMath.Core.Evaluation.LevelDifficulty.SelectionPolicies;
using CrossMath.Core.Evaluation.LevelDifficulty.WeightCalculators;
using CrossMath.Core.Evaluation.LocalDifficulty;
using CrossMath.Core.Models;
using CrossMath.Core.Utils.Progress;

namespace CrossMath.Core.Evaluation.LevelDifficulty;

public sealed class LevelDifficultyEvaluator
{
    private readonly LocalDifficultyEvaluator _localEvaluator;
    private readonly IScoreStrategy _scoreStrategy;
    private readonly IWeightCalculator _weightCalculator;
    private readonly ISelectionPolicy _selectionPolicy;
    private readonly IEvaluationTrace _trace;

    private readonly Func<Size, RowCol> _defaultInitLastCoordGetter;

    public LevelDifficultyEvaluator(
        LocalDifficultyEvaluator localEvaluator,
        IScoreStrategy scoreStrategy,
        IWeightCalculator weightCalculator,
        ISelectionPolicy selectionPolicy,
        IEvaluationTrace? trace = null,
        Func<Size, RowCol>? defaultInitLastCoordGetter = null)
    {
        _localEvaluator = localEvaluator;
        _scoreStrategy = scoreStrategy;
        _weightCalculator = weightCalculator;
        _selectionPolicy = selectionPolicy;
        _trace = trace ?? new NullEvaluationTrace();
        _defaultInitLastCoordGetter = defaultInitLastCoordGetter ?? (size => size.MaxCoord);
    }
    
    public static LevelDifficultyEvaluatorBuilder Create()
        => new();

    public static LevelDifficultyContext CreateContext(BoardData board)
    {
        return new LevelDifficultyContext(board);
    }

    public double Evaluate(LevelDifficultyContext ctx, int runCount, bool progress = true)
    {
        double mean = 0;

        using var tqdm = progress ? new Tqdm(runCount, "评估关卡难度") : null;

        for (int n = 1; n <= runCount; n++)
        {
            var sample = RunOnce(ctx, n - 1);   // 0-based step index
            mean += (sample - mean) / n;       // running mean: μₙ = μₙ₋₁ + (xₙ - μₙ₋₁) / n
            tqdm?.Update();
        }

        return Math.Round(mean, 3);
    }

    public double EvaluateSingleRun(LevelDifficultyContext ctx) => RunOnce(ctx, 0);
    
    private double RunOnce(LevelDifficultyContext ctx, int runIndex)
    {
        _trace.OnRunStart(runIndex);
        ctx.Reset();
      
        while (!ShouldStop(ctx))
        {
            // 1️⃣ 局部难度分析（你已经做得很好）
            var localDifficultyEvaluation =
                _localEvaluator.Evaluate(
                    LocalDifficultyEvaluator.CreateContext(ctx.WorkingBoard));
            var localDifficulties = localDifficultyEvaluation.MinDifficultyPerCell;
            
            var last = ctx.LastCoordinate ?? _defaultInitLastCoordGetter(ctx.WorkingBoard.BoardSize);
            // 2️⃣ 计算每个候选位置的 score
            var scoresMap = _scoreStrategy.Score(
                ctx,
                last,
                localDifficulties,
                localDifficultyEvaluation.CandidateMapAtCell
                );
            
            var weights = _weightCalculator.Calculate(ctx, scoresMap);

            RowCol chosen;
            try
            {
                // 3️⃣ 根据策略选择一个坐标
                chosen = _selectionPolicy.Select(weights);
            }
            catch (InvalidOperationException e)
            {
                Console.Error.WriteLine(e);
                var board = ctx.InitialBoard;
                var (encoded, layoutStr) = BoardDataCodec.Encode(board);
                Console.Error.WriteLine($"level: {encoded} \n layout: {layoutStr}");
                throw;
            }
            // // 3️⃣ 根据策略选择一个坐标
            // var chosen = _selectionPolicy.Select(weights);
            
            var step = new EvaluationStep(
                ctx.StepIndex,
                chosen,
                localDifficulties[chosen],
                scoresMap[chosen],
                ctx.WorkingBoard.PossibleAnswers.Distinct().Count(),
                new Dictionary<RowCol, int>(localDifficulties)
            );

            _trace.OnStepEvaluated(step);
            
            // 4️⃣ 推进状态
            ctx.ApplySelection(chosen, scoresMap[chosen]);
        }
        
        _trace.OnRunEnd(runIndex, ctx.Score);
        return ctx.Score;
    }

    private bool ShouldStop(LevelDifficultyContext ctx)
        => ctx.IsComplete();
    
}
