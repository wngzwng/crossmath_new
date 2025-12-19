// See https://aka.ms/new-console-template for more information

using CrossMath.Service.Jobs;
using CrossMath.Service.Pipelines;
using Microsoft.Extensions.Logging;

Console.WriteLine("Hello, World!");

// 测试

var loggerFacotry = LoggerFactory.Create(builder =>
{
    // builder.AddConsole();
    builder.AddDebug();
});
var pipeline = new BoardAnalysisPipeline(loggerFacotry);
var initBordJob = new InitBoardAnalysisJob(pipeline, 1000);

var inputfile = "/Users/admin/RiderProjects/Puzzle/CrossMath/data/merged_distinct_new_encode.csv";
var finalOutput = "/Users/admin/RiderProjects/Puzzle/CrossMath/data/merged_distinct_new_encode_difficulty.csv";
initBordJob.Run(inputfile, finalOutput);    