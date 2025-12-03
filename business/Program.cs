// See https://aka.ms/new-console-template for more information

using business.Converters;
using business.Csv;
using business.Records;
using business.utils;
using business.works;
using business.works.Layout;
using CrossMath.Core.Analytics.EmptyBoard;
using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Generators.PlacementGenerators;
using CrossMath.Core.Generators.StopPolicies;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

Console.WriteLine("Hello, World!");

//  使用 JobBatchFactory 创建 job




var stopPolicy = StopPolicyFactory.CategoryQuota(
    categorySelector: boardLayout =>
        ExpressionLayoutBuilder.ExtractLayouts(boardLayout, [5, 7])
            .Count(exprLayout => exprLayout.Length == 7),

    ratioMap: new Dictionary<int, double>
    {
        [1] = 0.8,
        [2] = 0.2,
    },

    totalCount: 3000
);

var tqdm = new TqdmProgressPrinter();
var targetSize = new Size(Height: 9, Width: 7);
var job  = new LayoutGenerationJob()
{
    CanvasSize = targetSize,
    MinFormulaCount = 5,
    MaxFormulaCount = 9,
    MaxSigma = 6.0,
    PlacementGenerator = new PlacementGenerator()
        .WithPlaceStrategies([(5, CrossType.Number), (7, CrossType.Number)])
        .StopAtFirstMatch(false),
    
    InitPlacements = InitialPlacementGenerator.BuildPlacement([
        (targetSize, 5, CrossType.Number),
        (targetSize, 7, CrossType.Number),
    ]),
    TargetCount = 30000,
    ProgressCallback = (cur, total) => tqdm.Report(cur, total),
    StopPolicy = stopPolicy,
};

LayoutGenerationPipeline.RunAndExport(job, "Generator Layouts 9x7");



// var stopPolicy = StopPolicyFactory.CategoryQuota(
//     categorySelector: boardLayout =>
//         ExpressionLayoutBuilder.ExtractLayouts(boardLayout, [5, 7])
//             .Count(exprLayout => exprLayout.Length == 7),
//
//     ratioMap: new Dictionary<int, double>
//     {
//         [1] = 0.8,
//         [2] = 0.2,
//     },
//
//     totalCount: 3000
// );
//
// var outputDir = CsvConfig.ResolvePath("full");
// Directory.CreateDirectory(outputDir);
//
// var jobs = LayoutJobBatchFactory.CreateJobsFromSpecs(targetCount: 30000, maxSigma: 6.0, stopPolicy);
// foreach (var job in jobs)
// {
//     var size = job.CanvasSize;
//     var layoutRecords = LayoutGenerationPipeline.Run(job, $"Generator Layouts {size.Height}*{size.Width}: ");
//     var layoutFullRecords = layoutRecords.Select(LayoutFullRecordFactory.FromLayoutRecord).ToList();
//     
//     CsvUtils.Write(Path.Combine(outputDir, $"layouts_{job.CanvasSize.Height}x{job.CanvasSize.Width}.csv"), layoutFullRecords);
// }




// var inputDir = CsvConfig.RootDirectory;
// var outputDir = CsvConfig.ResolvePath("data/full");
//
// // 创建输出目录（若不存在）
// Directory.CreateDirectory(outputDir);
//
// // 获取所有 CSV 输入文件
// var files = FileUtils.GetFiles(inputDir, ".csv").ToList();
//
// foreach (var file in files)
// {
//     var fileName = Path.GetFileNameWithoutExtension(file);
//     var outputFile = Path.Combine(outputDir, $"{fileName}_full.csv");
//
//     // 运行转换
//     CsvTransform.Transform<LayoutRecord, LayoutFullRecord>(
//         file,
//         outputFile,
//         (layoutRecord) =>
//         {
//             var id = layoutRecord.Id;
//             var boardLayout = new BoardLayout(layoutRecord.LayoutStr, layoutRecord.LayoutSize);
//             var fullRecord = EmptyBoardAnalyzer.GetInfo(boardLayout).ToFullRecord();
//             fullRecord.Id = id;
//             return fullRecord;
//         }
//     );
//
//     Console.WriteLine($"Converted: {file} → {outputFile}");
// }