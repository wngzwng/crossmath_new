using CrossMath.Core.Codec;
using CrossMath.Core.Evaluation.GlobalCellDifficulty;
using CrossMath.Core.Evaluation.LevelDifficulty;
using CrossMath.Core.Evaluation.LevelDifficulty.Analysis;
using CrossMath.Core.Evaluation.LevelDifficulty.EvaluationTraces.MaxWeight;
using CrossMath.Core.Evaluation.LevelDifficulty.ScoreStrategies;
using CrossMath.Core.Evaluation.LevelDifficulty.SelectionPolicies;
using CrossMath.Core.Evaluation.LevelDifficulty.WeightCalculators;
using CrossMath.Core.Evaluation.LocalDifficulty;
using CrossMath.Core.Models;
using CrossMath.Core.Utils;
using Microsoft.Extensions.Logging;

namespace CrossMath.Service.Pipelines;


public sealed class BoardAnalysis
{
    public BoardData Board { get;  }
    private readonly List<int> _globalDifficulties;

    private readonly List<int> _localDifficulties;
    private readonly MaxWeightPathAnalysis _maxWeightPathAnalysis;


    public IReadOnlyList<int> GlobalDifficulties => _globalDifficulties;
    
    public IReadOnlyList<int> LocalDifficulties => _localDifficulties;

    public int StuckCount => _maxWeightPathAnalysis.StuckNum;

    public IReadOnlyList<int> StuckPoints =>   _maxWeightPathAnalysis.StuckPoints
        .Select(i => i + 1)
        .ToList();
    
    public int FirstStuckPoint => _maxWeightPathAnalysis.FirstStuckPoint is int idx
        ? idx + 1
        : 0;

    public double FirstStuckPointPercent => _maxWeightPathAnalysis.FirstStuckPointPercent ?? 0;

    public double EmptyFriendly => NumberFriendlyCalculator.CalcFriendlyAverage(Board.GetAnswerNumbers());

    public IReadOnlyList<(int row, int col, int localDifficulty)> MaxWeightPath =>  _maxWeightPathAnalysis.PathCoordinates
        .Select(p => (p.Row + 1, p.Col + 1, p.LocalDifficulty))
        .ToList();
    
    public double MaxWeightLevelScore { get; }
    public double LevelScore { get; }
    

    public BoardAnalysis(BoardData board, List<int> globalDifficulties, List<int> localDifficulties, MaxWeightPathAnalysis maxWeightPathAnalysis, double maxWeightLevelScore, double levelScore)
    {
        Board = board;
        _globalDifficulties = globalDifficulties;
        _localDifficulties = localDifficulties;
        _maxWeightPathAnalysis = maxWeightPathAnalysis;
        MaxWeightLevelScore = maxWeightLevelScore;
        LevelScore = levelScore;
    }
}

public class BoardAnalysisPipeline
{
    private readonly GlobalDifficultyEvaluator _globalDifficultyEvaluator;
    private readonly LocalDifficultyEvaluator _localDifficultyEvaluator;
    private readonly LevelDifficultyEvaluator _levelDifficultyEvaluator;
    private readonly LevelDifficultyEvaluator _maxWeightLevelDifficultyEvaluator;
    private readonly MaxWeightPathTrace _maxWeightPathTrace;

    public BoardAnalysisPipeline(ILoggerFactory loggerFactory)
    {
        _globalDifficultyEvaluator = GlobalDifficultyEvaluator.CreateDefault(loggerFactory);
        _localDifficultyEvaluator = LocalDifficultyEvaluator.CreateDefault(loggerFactory);
        _maxWeightPathTrace = new MaxWeightPathTrace();
        
        _levelDifficultyEvaluator = LevelDifficultyEvaluator.Create()
            .WithLocalEvaluator(_localDifficultyEvaluator)
            .WithScoreStrategy(new DefaultScoreStrategy())
            .WithSelectionPolicy(new WeightedRandomSelectionPolicy())
            .WithWeightCalculator(new ExponentialWeightCalculator())
            .Build();

        _maxWeightLevelDifficultyEvaluator = LevelDifficultyEvaluator.Create()
            .WithLocalEvaluator(_localDifficultyEvaluator)
            .WithScoreStrategy(new DefaultScoreStrategy())
            .WithSelectionPolicy(new MaxWeightSelectionPolicy())
            .WithWeightCalculator(new ExponentialWeightCalculator())
            .WithTrace(_maxWeightPathTrace)
            .Build();
    }
    public BoardAnalysis Run(BoardData board, int levelRunCount)
    {
        // board.PrettyPrint();
        MaxWeightPathAnalysis maxWeightPathAnalysis = BuildMaxWeight(board.Clone(), out var maxWeightScore);

        return new BoardAnalysis(
            board: board,
            globalDifficulties: BuildGlobalDifficulty(board.Clone()),
            localDifficulties: BuildLocalDifficulty(board.Clone()),
            maxWeightPathAnalysis: maxWeightPathAnalysis,
            maxWeightLevelScore: maxWeightScore,
            levelScore: BuildLevelScore(board.Clone(), levelRunCount)
        );
    }


    public List<int> BuildGlobalDifficulty(BoardData board)
    {
        var globalDifficulties =
            _globalDifficultyEvaluator.Evaluate(GlobalDifficultyEvaluator.CreateContext(board));

        return globalDifficulties.OrderBy(x => x.Key).Select(kv => kv.Value).ToList();
    }

    public List<int> BuildLocalDifficulty(BoardData board)
    {
        var localDifficulties =
            _localDifficultyEvaluator.EvaluateMinDifficulty(LocalDifficultyEvaluator.CreateContext(board));
        
        return localDifficulties.OrderBy(x => x.Key).Select(kv => kv.Value).ToList();
    }

    public MaxWeightPathAnalysis BuildMaxWeight(BoardData board, out double maxWeightScore)
    {
        maxWeightScore = _maxWeightLevelDifficultyEvaluator.EvaluateSingleRun(LevelDifficultyEvaluator.CreateContext(board));
        var maxWeightPath = _maxWeightPathTrace.Path;
        return MaxWeightPathAnalyzer.Analyze(maxWeightPath);
    }

    public double BuildLevelScore(BoardData board, int runCount)
    {
        return _levelDifficultyEvaluator.Evaluate(LevelDifficultyEvaluator.CreateContext(board.Clone()), runCount, false);
    }
}