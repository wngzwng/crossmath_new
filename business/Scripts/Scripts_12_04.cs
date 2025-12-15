using business.Converters;
using business.Csv;
using business.Records;
using business.utils;
using business.works;
using business.works.Layout;
using CrossMath.Core.Analytics.EmptyBoard;
using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Generators.Collectors;
using CrossMath.Core.Generators.CompletionCheckers;
using CrossMath.Core.Generators.PlacementGenerators;
using CrossMath.Core.Generators.StopPolicies;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace business.Scripts;

public static class Scripts_12_04
{
    public static void Run()
    {
        // See https://aka.ms/new-console-template for more information


        Console.WriteLine("Hello, World!");

        //  使用 JobBatchFactory 创建 job




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
        //     totalCount: 30000
        // );
        //
        // var tqdm = new TqdmProgressPrinter();
        // var targetSize = new Size(Height: 9, Width: 9);
        // var job  = new LayoutGenerationJob()
        // {
        //     CanvasSize = targetSize,
        //     MinFormulaCount = 5,
        //     MaxFormulaCount = 10,
        //     MaxSigma = 6.0,
        //     PlacementGenerator = new PlacementGenerator()
        //         .WithPlaceStrategies([(7, CrossType.Number), (5, CrossType.Number)])
        //         .StopAtFirstMatch(false),
        //     
        //     InitPlacements = InitialPlacementGenerator.BuildPlacement([
        //         (targetSize, 5, CrossType.Number),
        //         (targetSize, 7, CrossType.Number),
        //     ]),
        //     TargetCount = 30000,
        //     ProgressCallback = (cur, total) => tqdm.Report(cur, total),
        //     StopPolicy = stopPolicy,
        // };
        //
        // LayoutGenerationPipeline.RunAndExport(job, "layouts_9x9.csv","Generator Layouts 9x9");
        //



        var outputDir = CsvConfig.ResolvePath("full");
        Directory.CreateDirectory(outputDir);

        var targetCount = 30000;

        var bucketCounterFactory = new Func<int, BucketCounter<int>>(tc => new BucketCounter<int>(
            categorySelector: boardLayout =>
                ExpressionLayoutBuilder.ExtractLayouts(boardLayout, [5, 7])
                    .Count(exprLayout => exprLayout.Length == 7),
            ratioMap: new Dictionary<int, double>
            {
                [1] = 0.8,
                [2] = 0.2,
            },
            totalTargetCount: tc
        ));

        var jobs = LayoutJobBatchFactory.CreateJobsFromSpecs(
            targetCount: targetCount,
            maxSigma: 6.0,
            stopPolicyFactory: (_, counter) =>
                StopPolicyFactory.CreateComposite().Bucket(counter).Build(),
            completionCheckerFactory: (job, counter) => new CompletionChecker()
                .AddSizeFilter(size => size.Equals(job.CanvasSize))
                .AddFormulaCountFilter(cnt => job.MinFormulaCount <= cnt && cnt <= job.MaxFormulaCount)
                .AddSigmaFilter(sigma => sigma <= job.MaxSigma)
                // .AddCustomFilter(canvas =>  canvas.CountSevens() is 1 or 2)
                .AddBuckerFilter<int>(counter),   // 直接传 counter，不再用 job?.counter
            expandControllerFactory: (job, counter) =>
            {
                var expandController = new FormulaCountExpandController(
                    new Dictionary<Size, int>
                    {
                        { job.CanvasSize, job.MaxFormulaCount }
                    }
                );
                expandController.MostExp7Count = 1;
                if (counter != null)
                {
                    counter.BucketCompleted += (key, quote) => expandController.MostExp7Count = key + 1;
                }
                return expandController;
            },
            bucketCounterFactory: bucketCounterFactory
        );

        foreach (var job in jobs)
        {
            var size = job.CanvasSize;
            var layoutRecords = LayoutGenerationPipeline.Run(job, $"Generator Layouts {size.Height}*{size.Width}: ");
            var layoutFullRecords = layoutRecords.Select(LayoutFullRecordFactory.FromLayoutRecord).ToList();
            
            CsvUtils.Write(Path.Combine(outputDir, $"layouts_{job.CanvasSize.Height}x{job.CanvasSize.Width}.csv"), layoutFullRecords);
        }

        /*
         * 数量影响了这个生成规律，而且是在 20%左右，这个数量影响了什么
        1. 生成点，排序，不受数量影响
        2. 收集问题
        Generator Layouts 11*11: : 26%|███████░░░░░░░░░░░░░░░░░░░░░░░| 78184/300000 [40s<02m53s, 1968.6it/s]
            1: 18185/240000  │  [Success]2: 60000/60000
                   0   1   2   3   4   5   6   7   8   9  10 
           -------------------------------------------------
              0 |  □   ◇   □   =   □                         
              1 |          ◇       ◇                         
              2 |  □   ◇   □   =   □                         
              3 |  ◇       =       =                         
              4 |  □   ◇   □   =   □       □   ◇   □   =   □ 
              5 |  =                               ◇       ◇ 
              6 |  □   ◇   □   =   □       □   ◇   □   =   □ 
              7 |                  ◇       ◇       =       = 
              8 |                  □   ◇   □   ◇   □   =   □ 
              9 |                  =       =                 
             10 |  □   ◇   □   =   □       □   ◇   □   =   □ 
           -------------------------------------------------
           
           问题解决：
           当某个分类收集完成时，告之拓展器做出对应的行为即可，避免在无效分支中深度漫游
         */


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
    }
}