// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using business;
using business.Csv;
using business.Scripts;
using CrossMath.Core.Analytics.FinalBoard;
using CrossMath.Core.Codec;
using CrossMath.Core.Models;

// RootCommand root = new("Final board generator");
//
// // 选项
// var inputOption = new Option<string?>("--input", "输入文件路径（文件模式）");
// var inputDirOption = new Option<string?>("--input-dir", "输入目录路径（目录模式）")
// {
//     DefaultValueFactory = _ => $"{Path.Combine(CsvConfig.RootDirectory, "split")}"
// };
// var outputOption = new Option<string?>("--output", "输出文件路径（文件模式）");
// var outputDirOption = new Option<string?>("--output-dir", "输出目录路径（目录模式）")
// {
//     DefaultValueFactory = _ => $"{Path.Combine(CsvConfig.RootDirectory, "split_output")}"
// };
//
// var limitOption = new Option<int?>("--limit")
// {
//     Description = "处理上限",
//     DefaultValueFactory = _ => 5
// };
//
// var parallelOption = new Option<int?>("--parallel", "并行数")
// {
//     DefaultValueFactory = _ => 5
// };
//
// // 注册选项
// root.Options.Add(inputOption);
// root.Options.Add(inputDirOption);
// root.Options.Add(outputOption);
// root.Options.Add(outputDirOption);
// root.Options.Add(limitOption);
// root.Options.Add(parallelOption);
//
// // 设置执行行为（官方写法）
// root.SetAction(async parseResult =>
// {
//     string? input     = parseResult.GetValue(inputOption);
//     string? inputDir  = parseResult.GetValue(inputDirOption);
//     string? output    = parseResult.GetValue(outputOption);
//     string? outputDir = parseResult.GetValue(outputDirOption);
//     int? limit         = parseResult.GetValue(limitOption);
//     int? parallel     = parseResult.GetValue(parallelOption);
//     
//     await FinalBoardCmd.Run(input, inputDir, output, outputDir, limit, parallel);
// });
//
// // 运行（官方写法）
// return root.Parse(args).Invoke();

/*
./bin/Debug/net9.0/business --input /Users/admin/RiderProjects/PuzzrossMath/config/保留面板.csv --output /Users/admin/RiderProjects/Puzzle/CrossMath/bu
   Math/business/Data/fillBoard.csv --limit 3
   
   
   # 1. 最推荐：带进度条 + 可中断恢复 + 输出文件名加 .processed.csv 后缀
find /Users/admin/RiderProjects/Puzzle/CrossMath/business/Data/split -maxdepth 1 -name "*.csv" -type f | \
parallel --eta --bar --joblog log.txt --resume --retry-failed -j 100% \
     ./business --input {} --output /Users/admin/RiderProjects/Puzzle/CrossMath/business/Data/split_output/{/.}.processed.csv --limit 5
   */

// var intput = "/Users/admin/Desktop/split/crossmath_filledBoards_1210.csv";
// var output = Path.Combine(CsvConfig.RootDirectory, "filledBoards_1211.csv");
// Scripts_12_09.Run(intput, output); 

var board = BoardDataCodec.Decode("750d0b05fdfefb02fd0bfa16fdfbfa01fd1bfa1bfafa1a1c", "10101101011111110101111111010010100");
board.PrettyPrint();

var analyzer = new FinalBoardAnalyzer();
Console.WriteLine(analyzer.CountOneTwoMinOperatorInExp7(board));