using business.Records;
using business.utils;
using business.works.Hole;
using CrossMath.Core.Analytics.FinalBoard;
using CrossMath.Core.Codec;
using CrossMath.Core.ExpressionSolvers;
using CrossMath.Core.ExpressionSolvers.ExpressionValidators;
using CrossMath.Core.ExpressionSolvers.NumberPools;
using CrossMath.Core.ExpressionSolvers.OperatorPools;
using CrossMath.Core.ExpressionSolvers.SolverProviders;
using CrossMath.Core.Fillers;
using CrossMath.Core.Models;
using CrossMath.Core.Types;
using CrossMath.Core.Utils;

namespace business.Scripts;

public static class Scripts_12_17
{
    // 终盘分析器
    private static readonly HoleRunner _runner = new HoleRunner();


    // =====================================================
    // 主流程入口：可直接用于批处理
    // =====================================================
    public static void Run(string inputFile, string outputFile)
    {
        Console.WriteLine("=== Generate Final Boards ===");

        // ============================================================
        // 1. 读取空盘分析结果（CSV → Dictionary）
        // ============================================================
        var filledBoards = CsvUtils.ReadDictCsv(inputFile).ToList();

        // ============================================================
        // 处理每一个空盘
        // ============================================================
        var bar = new TqdmProgressPrinter($"hole board {Path.GetFileNameWithoutExtension(inputFile)}");
        var curIndex = 0;
        bar.Report(curIndex, filledBoards.Count);

        var initBoards = new List<OrderedDictionary<string, object?>>();
        foreach (var filledBoard in filledBoards)
        {
            var layoutStr = (string)filledBoard["layout_info"]!;
            var level = (string)filledBoard["end_info"]!;

            var board = BoardDataCodec.Decode(level, layoutStr);
            foreach (var initBoard in _runner.IterHollowOutBoard(board))
            {
                var answer = string.Join(",", initBoard.PossibleAnswers);
                var (levelStr, layout) = BoardDataCodec.Encode(initBoard);
                var record = new OrderedDictionary<string, object?>(filledBoard);
                record["answer"] = answer;
                record["start_info"] = levelStr;
                record["cell_num_empty"] = initBoard.Holes.Count;
                
                initBoards.Add(record);
            }
            bar.Report(++curIndex, filledBoards.Count);
        }

        // ============================================================
        // 输出最终 CSV
        // ============================================================
        CsvUtils.WriteDictCsv(
            outputFile,
            initBoards
        );

        Console.WriteLine($"Saved {initBoards.Count} final boards to init_boards.csv");
        Console.WriteLine("=== Finish ===");
    }
}
