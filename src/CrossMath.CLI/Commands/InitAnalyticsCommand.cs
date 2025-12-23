using System.CommandLine;
using System.Security.Cryptography;
using CrossMath.CLI.Framework;
using CrossMath.Service.Hole;
using CrossMath.Service.Jobs;
using CrossMath.Service.Pipelines;
using CrossMath.Service.Utils;
using CrossMath.Service.Utils.Progress;
using Microsoft.Extensions.Logging;

namespace CrossMath.CLI.Commands;

public class InitAnalyticsCommand : CliCommandBase
{
    private readonly Option<string> _inputFile =
        new("--input", "-i")
        {
            Description = "输入文件(.csv)",
            Required = true
        };

    private readonly Option<string?> _outputFile =
        new("--output", "-o")
        {
            Description = "输出文件(.csv)",
            DefaultValueFactory = _ => null
        };

    private readonly Option<bool> _progress =
        new("--progress", "-p")
        {
            Description = "是否进度条",
            DefaultValueFactory = _ => false
        };

    private readonly Option<string?> _outputDir =
        new("--output-dir", "-d")
        {
            Description = "输出目录",
            DefaultValueFactory = _ => null
        };
    
    private readonly Option<int> _levleRunCount =
        new("--run-count")
        {
            Description = "输出目录",
            DefaultValueFactory = _ => 1000
        };
    
    public InitAnalyticsCommand() : base("analytics-init", "初盘分析") { }
    protected override void Configure(Command command)
    {
        command.Add(_inputFile);
        command.Add(_outputFile);
        command.Add(_outputDir);
        command.Add(_progress);
        command.Add(_levleRunCount);
    }

    protected override Task<int> HandleAsync(ParseResult parse)
    {
        string input = parse.GetValue(_inputFile);
        string? output = parse.GetValue(_outputFile);
        string? outputDir = parse.GetValue(_outputDir);
        bool needProgress = parse.GetValue(_progress);
        int runCount = parse.GetValue(_levleRunCount);

        if (string.IsNullOrWhiteSpace(output) && string.IsNullOrWhiteSpace(outputDir))
        {
            var errorCode = CliErrors.Error(
                "必须指定 --output 或 --output-dir 其中之一");
            return Task.FromResult(errorCode);
        }

        if (!string.IsNullOrWhiteSpace(output) && !string.IsNullOrWhiteSpace(outputDir))
        {
            var errorCode =  CliErrors.Error(
                "--output 与 --output-dir 不能同时使用");
            return Task.FromResult(errorCode);
        }
        
        string finalOutput;

        if (!string.IsNullOrWhiteSpace(output))
        {
            finalOutput = output;
        }
        else
        {
            Directory.CreateDirectory(outputDir!);
            var inputFileName = Path.GetFileName(input);
            finalOutput = Path.Combine(outputDir!, inputFileName);
        }
        
        
        var loggerFacotry = LoggerFactory.Create(builder =>
        {
            // builder.AddConsole();
            builder.AddDebug();
        });
        var pipeline = new BoardAnalysisPipeline(loggerFacotry);
        var initBordJob = new InitBoardAnalysisJob(pipeline, runCount, needProgress ? new Core.Utils.Progress.StdOutProgressWriter() : null);
        initBordJob.Run(input, finalOutput);
        return Task.FromResult(0);
    }
}