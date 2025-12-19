// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using CrossMath.Core.Analytics.EmptyBoard;
using CrossMath.Core.BoardSolvers;
using CrossMath.Core.CandidateDomains;
using CrossMath.Core.Codec;
using CrossMath.Core.Evaluation;
using CrossMath.Core.Evaluation.GlobalCellDifficulty;
using CrossMath.Core.Evaluation.LevelDifficulty;
using CrossMath.Core.Evaluation.LevelDifficulty.Analysis;
using CrossMath.Core.Evaluation.LevelDifficulty.EvaluationTraces.MaxWeight;
using CrossMath.Core.Evaluation.LevelDifficulty.ScoreStrategies;
using CrossMath.Core.Evaluation.LevelDifficulty.SelectionPolicies;
using CrossMath.Core.Evaluation.LevelDifficulty.WeightCalculators;
using CrossMath.Core.Evaluation.LocalDifficulty;
using CrossMath.Core.Expressions.Core;
using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.ExpressionSolvers;
using CrossMath.Core.ExpressionSolvers.Expression5Solvers;
using CrossMath.Core.ExpressionSolvers.Expression7Solvers;
using CrossMath.Core.ExpressionSolvers.ExpressionValidators;
using CrossMath.Core.ExpressionSolvers.NumberPools;
using CrossMath.Core.ExpressionSolvers.OperatorPools;
using CrossMath.Core.ExpressionSolvers.SolverProviders;
using CrossMath.Core.Fillers;
using CrossMath.Core.Generators;
using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Generators.PlacementGenerators;
using CrossMath.Core.HoleDiggers;
using CrossMath.Core.HoleDiggers.HoleCount;
using CrossMath.Core.HoleDiggers.HoleVadidators;
using CrossMath.Core.HoleDiggers.HollowOutStrategies;
using CrossMath.Core.Models;
using CrossMath.Core.Types;
using CrossMath.Core.Utils;
using CrossMath.Core.Utils.Progress;
using Microsoft.Extensions.Logging;

Console.WriteLine("Hello, World!");

// var layout_str = "1111101000010111111011010101101111110100001011111";
// var (height, width)  =  (7, 7);
// var boardLayout = new BoardLayout(layout_str, width: width, height: height);
// boardLayout.PrettyPrint('*');

// var exprLayouts = ExpressionLayoutBuilder.ExtractLayouts(boardLayout, [5]);
// foreach (var exprLayout in exprLayouts)
// {
//     Console.WriteLine(exprLayout);
// }

// var board = BoardDataCodec.Decode(layout_str, height, height);
// board.PrettyPrint();

// var level = "9b00310efb00fa0ffcfcfbfb00fc00fa0000fbfafafa19fb17fa0000fb14fa3cfcfafb00fc0dfa0000fafa00fd09fa481611011b011a2d302811040208";
// var layout = "001010111110010101000100111110001001010100011111101111110100000100111110001001000000010011111000100";

// var level = "9b00310efb00fa0ffcfcfbfb00fc00fa0000fbfafafa19fb17fa0000fb14fa3cfcfafb00fc0dfa0000fafa00fd09fa481611011b011a2d302811040208";
// var layout = "001010111110010101000100111110001001010100011111101111110100000100111110001001000000010011111000100";
// var boardLayout = new BoardLayout(layout, width: 11, height: 9);
// // var level = "7500fd01fa00fbfb000c03fafbfa00fc02fa12fa000f0f05140e";
// // var layout = "11111100011010110101111110010000100";
// var board = BoardDataCodec.Decode(boardLayout);
// board.PrettyPrint();
//
// var solver5 = new Expression5Solver();
// var solver7 = new Expression7Solver();
// var ctx = new ExpressionSolveContext()
// {
//     NumPool = NumberPoolFactory.Create(board.GetAnswerNumbers()),
//     OpPool = OperatorPoolFactory.ASMD,
//     Validator = new ExpressionValidator(ValidationMode.FullDiscreteConsume)
// };

// var exp7 = ExpressionFactory.FromArray(["", "/", "", "*", "", "=", "20"]);
// var solutions = solver.Solve(exp7, ctx);
// foreach (var solution in solutions)
// {
//     Console.WriteLine(solution);
// }

// var exprLayouts = ExpressionLayoutBuilder.ExtractLayouts(board.Layout, [5, 7]);
// foreach (var exprLayout in exprLayouts)
// {
//     Console.WriteLine(exprLayout);
//     var expr = exprLayout.ToExpression(board);
//     Console.WriteLine($"{expr} number: {expr.EmptyNumberCount()} operator: {expr.EmptyOperatorCount()}");
//     var solutions = expr.Length == 5 ? solver5.Solve(expr, ctx) : solver7.Solve(expr, ctx);
//     foreach (var solution in solutions)
//     {
//         Console.WriteLine(solution);
//     }
// }



// Console.WriteLine(3.0 / 4.0 * 4.0 == 3.0);

// var layout = "0010111111100101010101111110111111010101010111111011111100000000001111101000000101010000001111100000010101000011111011111";
// var boardLayout = new BoardLayout(layout, width: 11, height: 11);
//
// var provider = ExpressionSolverProvider.CreateDefault();
// var solvedCtx = new ExpressionSolveContext()
// {
//     NumPool = NumberPoolFactory.Create(1, 20, NumberOrder.Shuffled),
//     OpPool = OperatorPoolFactory.MDAS,
//     Validator = new ExpressionValidator(ValidationMode.FullPoolCheck)
// };
// //
// var filter = new LayoutFiller(provider)
//     .Setup(solvedCtx)
//     .WithSolutionSampleLimit(20)
//     .WithFirstFillMode(FirstFillSelectMode.Random);
//
// if (filter.TryFill(boardLayout, 1000, [5, 7], out var board, out var successIndex))
// {
//     board!.PrettyPrint();
//     Console.WriteLine($"success: {successIndex}");
// }


// normal layoutGenrator
// var canvas = new LayoutCanvas(20, 20);
// canvas.TryApplyPlacement(new Placement(10, 7, Direction.Horizontal, 5), out var _);
//
// canvas.ExportBoardLayout(false).LogicPrettyPrint();
// var placementGenrator = new PlacementGenerator()
//     .WithPlaceStrategies([
//         (5, CrossType.Number),
//     ]);
//
// var layoutGenerator = new LayoutGenerator(placementGenrator);
// var layout = layoutGenerator.Generator(canvas);
// if (layout != null)
// {
//     layout.Value.LogicPrettyPrint();
//     Console.WriteLine(layout.Value);
//     if (filler.TryFill(layout.Value, 100, [5, 7], out var board, out var successIndex))
//     {
//         board!.PrettyPrint();
//         Console.WriteLine($"success: {successIndex}");
//     }
//     else
//     {
//         Console.WriteLine("Failed to fill board");
//     }
// }
// smart layoutGenerator
// var canvas = new LayoutCanvas(11, 11);
// canvas.TryApplyPlacement(new Placement(0, 0, Direction.Horizontal, 5), out var _);
//
// var ctx = new LayoutGenContext();

// var layoutGenerator = new LayoutGeneratorSimple();
// foreach (var layout in layoutGenerator.Generate(canvas, ctx, 10 ))
// {
//     layout.LogicPrettyPrint();
//     Console.WriteLine($"{layout.Sigma()}");
// }

// min=1, max=1 difficulty:11111    
// var level_1 = "7701fd00fa09fdfb001ffc00fa00fafcfafb1300151afafa092d09130c1316";
// var layout_1 = "1111100100010010111111010101101010100100010010001";
//
// //min=1, max = 2  difficulty: 1222222122
// var level_2 = "7726fe00fa02fc0000fb00fa2dfcfafc00fb00fa0000fafa00fe00fa0b0b132f1c11190a23221602";
// var layout_2 = "0011111001000010111111010001111110110000011111101";
//
// // min=1, max=3 difficulty: 3332223221   3,3,3,2,2,2,3,2,2,1
// var level_3 = "7700fb00fa55fbfc0000fc00fa00fafcfa0000fb00fa78fa00fe07fa080352756e135b78364238";
// var layout_3 = "1111100100010010111111010100101111100100000011111";
//
// // min = 1, max=4  difficulty: 3344241233
// var level_4 = "7700fb00fa13fbfc05fc00fa000ffbfafa000000fa00fb00fa001201020302040407040b";
// var layout_4 = "0011111000010111111011000101100010110000001111100";
//
// // min = 3, max = 4 difficulty: 333 444 333 4 33444444444444    3,3,3,4,4,4,3,3,3,4,3,3,4,4,4,4,4,4,4,4,4,4,4,4
// // 
// var level_5 = "bb00fd08fa000000fbfbfd000000fc00fa00fcfcfafafa000efb00fa0000fafa00fd00fa0000fbfc0000fd00fa00fafa00fc00fa00010817191810050401170d1b190102020712050414141202";
// var layout_5 = "0011111010100000010101100010111111000101010110001111101100010000001111101000000100010000001000111110010001000000111110000";
//
// var board = BoardDataCodec.Decode(level_5, layout_5);
// board.PrettyPrint();
//
// // 创建 LoggerFactory
// using var loggerFactory = LoggerFactory.Create(builder =>
// {
//     builder.AddConsole();  // 添加控制台日志提供者
//     builder.AddDebug();   // 添加调试输出日志提供者
//     // 可以添加其他日志提供者，如文件日志等
// });
//
// // 难度评估
// var evaluator = new GlobalDifficultyEvaluator([
//         new GlobalDifficultyLayerOne(),
//         new GlobalDifficultyLayerTwo(),
//         new GlobalDifficultyLayerThree(),
//     ], loggerFactory);
//
// List<KeyValuePair<RowCol, int>> result = null;
// using (PerformanceScope.Measure("难度评估"))
// {
//     result = evaluator.Evaluate(evaluator.CreateContext(board)).OrderBy(x => x.Key).ToList();
// }
// foreach (var pair in result)
// {
//     Console.WriteLine($"{pair.Key},{pair.Value}");
// }
// var resultList = result.Select(x => x.Value).ToList();
// Console.WriteLine($"{string.Join(",", resultList)}");
//
// var boardSolver = new BoardSolver();
// var counter = 0;
//
// using var scope = new PerformanceScope("求解");
// foreach (var solution in boardSolver.Solve(board, boardSolver.CreateDefaultExpressionSolverProvider()))
// {
//     Console.WriteLine($"========{++counter}============");
//     var answers = solution.solutionMap.OrderBy(pair => pair.Key).ToList();
//     Console.WriteLine(string.Join(",", answers.Select(pair => pair.Value)));
//     // foreach (var keyValuePair in answers)
//     // {
//     //     Console.WriteLine($"{keyValuePair.Key},{keyValuePair.Value}");
//     // }
// }

// // 四象限点的划分 
// var size3x3 = new Size(3, 3);
// var size4x4 = new Size(4, 4);
// foreach (var (start, end) in  size4x4.GetQuadrants())
// {
//     Console.WriteLine($"start: {start}, end: {end}");
// }


// var level = "bb67fb04fb04fa6ffbfbfc11fb48fa5914fafafa784cfb0ffa5b0afbfb2720fb09fa2901fbfbfafbfb09fb03fa0c18fb3bfa53fafafafa302cfb38fa645e";
// var layout = "0011111110000101000100001111101000010100010000101111101000000100011000111110110001010101111110111111000100010110001111101";

var level = "bb00fc00fa00fcfb0000fb00fa00fafafe1700fd00fa00fa00fe00fa00fc00fa01fd00fa00261c0a0f12182a2201222a012a1c0e0e";
var layout = "0011111000000100010000001011111000010001010000100011111000000001000000111110000001000000000010000000000100000011111000000";

var borad = BoardDataCodec.Decode(level, layout);
borad.PrettyPrint();

var (encoded, layout_str) = BoardDataCodec.Encode(borad);
Console.WriteLine(encoded);
Console.WriteLine(layout_str);
// var holeCountType = RandomHoleCountTypeSelector.GetRandomByDefaultWeight();
using var loggerFactory = LoggerFactory.Create(builder =>
{
    // builder.AddConsole();  // 添加控制台日志提供者
    builder.AddDebug();   // 添加调试输出日志提供者
    // 可以添加其他日志提供者，如文件日志等
});
var globalEvaluator = GlobalDifficultyEvaluator.CreateDefault(loggerFactory);
var localEvaluator = LocalDifficultyEvaluator.CreateDefault(loggerFactory);
var maxWeightPathTrace = new MaxWeightPathTrace();
var levelEvaluator = LevelDifficultyEvaluator.Create()
    .WithLocalEvaluator(localEvaluator)
    .WithScoreStrategy(new DefaultScoreStrategy())
    .WithWeightCalculator(new ExponentialWeightCalculator())
    .WithSelectionPolicy(new MaxWeightSelectionPolicy())
    .WithTrace(maxWeightPathTrace)
    .Build();

var holeCountType = HoleCountType.FormulaCountMinus1;
var ctx = HollowOutContext.Create(
    borad, holeCountType, 
    HollowOutStrategyFactory.CreateStrategy(HollowOutStrategyType.NonFocusPriority), 
    DefaultHoleValidator.Create());
var holeDigger = new HoleDigger();
Console.WriteLine("========");
if (holeDigger.TryHollowOut2(ctx, out var resultBoard))
{
    resultBoard.PrettyPrint();
    return 0;
    
}

// resultBoard = borad;
// resultBoard.PrettyPrint();
// var result = globalEvaluator.Evaluate(GlobalDifficultyEvaluator.CreateContext(resultBoard)).OrderBy(x => x.Key).ToList();
// Console.WriteLine("global result");
// foreach (var pair in result)
// {
//     Console.WriteLine($"{pair.Key},{pair.Value}");
// }
//
// var localResult = localEvaluator.EvaluateMinDifficulty(LocalDifficultyEvaluator.CreateContext(resultBoard)).OrderBy(x => x.Key).ToList();
// Console.WriteLine("localResult");
// foreach (var pair in localResult)
// {
//     Console.WriteLine($"{pair.Key},{pair.Value}");
// }
//
// var sw = Stopwatch.StartNew();
// var levelScore = levelEvaluator.Evaluate(LevelDifficultyEvaluator.CreateContext(resultBoard), 1000);
// sw.Stop();
//
// Console.WriteLine(
//     $"levelScore: {levelScore}, elapsed: {sw.Elapsed.TotalSeconds:F2} s"
// );
//
// var maxWeightPathAnalysis = MaxWeightPathAnalyzer.Analyze(maxWeightPathTrace.Path);
// Console.WriteLine($"{maxWeightPathAnalysis.ToDebugString()}");

return 0;
// using (var tqdm = ProgressBarUtils.Create(100, "进度条测试"))
// {
//     foreach (var i in Enumerable.Range(0, 100))
//     {
//         Thread.Sleep(10);
//         tqdm.Update();            
//     }
// }

/*
./CrossMath.CLI hole -i /Users/admin/RiderProjects/Puzzle/CrossMath/data/test/filledBoards_part0001.csv -o ~/RiderProjects/Puzzle/CrossMath/data/test_0001.csv
*/