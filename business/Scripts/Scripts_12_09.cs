using business.Records;
using business.utils;
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

public static class Scripts_12_09
{
    // 终盘分析器
    private static readonly FinalBoardAnalyzer finalBoardAnalyzer = new FinalBoardAnalyzer();


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
        var bar = new TqdmProgressPrinter($"fill board {Path.GetFileNameWithoutExtension(inputFile)}");
        var curIndex = 0;
        bar.Report(curIndex, filledBoards.Count);
        foreach (var filledBoard in filledBoards)
        {
            var layoutStr = (string)filledBoard["layout_info"]!;
            var level = (string)filledBoard["end_info"]!;

            var board = BoardDataCodec.Decode(level, layoutStr);
            var mixOperatorInExp7Count = finalBoardAnalyzer.CountOneTwoMinOperatorInExp7(board);
            filledBoard["has_one_two_mix_operator_in_exp7"] = mixOperatorInExp7Count > 0 ? 1 : 0;
            filledBoard["count_one_two_mix_operator_in_exp7"] = mixOperatorInExp7Count;
            bar.Report(++curIndex, filledBoards.Count);
        }

        // ============================================================
        // 输出最终 CSV
        // ============================================================
        CsvUtils.WriteDictCsv(
            outputFile,
            filledBoards
        );

        Console.WriteLine($"Saved {filledBoards.Count} final boards to final_boards.csv");
        Console.WriteLine("=== Finish ===");
    }
}
