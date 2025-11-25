// See https://aka.ms/new-console-template for more information

using CrossMath.Core.Codec;
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
using CrossMath.Core.Models;
using CrossMath.Core.Types;

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

// var layout = "001010111110010101000100111110001001010100011111101111110100000100111110001001000000010011111000100";
// var boardLayout = new BoardLayout(layout, width: 11, height: 9);
//
var provider = ExpressionSolverProvider.CreateDefault();
var solvedCtx = new ExpressionSolveContext()
{
    NumPool = NumberPoolFactory.Create(1, 20, NumberOrder.Shuffled),
    OpPool = OperatorPoolFactory.MDAS,
    Validator = new ExpressionValidator(ValidationMode.FullPoolCheck)
};
//
var filler = new LayoutFiller(provider)
    .Setup(solvedCtx)
    .WithSolutionSampleLimit(20)
    .WithFirstFillMode(FirstFillSelectMode.Random);

// fillter.SetSolutionSampleLimit(100);
// if (fillter.TryFill(boardLayout, 100, out var board, out var successIndex))
// {
//     board!.PrettyPrint();
// }

var canvas = new LayoutCanvas(20, 20);
canvas.TryApplyPlacement(new Placement(10, 7, Direction.Horizontal, 5), out var _);

canvas.ExportBoardLayout(false).LogicPrettyPrint();
var layoutGenerator = new LayoutGenerator(new PlacementGenerator());
var layout = layoutGenerator.Generator(canvas, [
    (7, CrossType.Operator),
    (5, CrossType.Number),
    (7, CrossType.Number),
    // (5, CrossType.Operator),
]);
if (layout != null)
{
    layout.Value.LogicPrettyPrint();
    Console.WriteLine(layout.Value);
    if (filler.TryFill(layout.Value, 100, [5, 7], out var board, out var successIndex))
    {
        board!.PrettyPrint();
        Console.WriteLine($"success: {successIndex}");
    }
    else
    {
        Console.WriteLine("Failed to fill board");
    }
}