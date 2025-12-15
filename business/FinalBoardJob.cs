using System.Diagnostics;
using business.Scripts;
using business.utils;

namespace business;

public static class FinalBoardJob
{
    public static void Run(string inputFile, string outputFile, int? limit)
    {
        Console.WriteLine($"[File] {inputFile}");
        Scripts_12_08.Run(inputFile, outputFile, limit);
    }

    public static void RunDirectory(string inputDir, string outputDir, int? limit)
    {
        Directory.CreateDirectory(outputDir);

        foreach (var file in Directory.GetFiles(inputDir, "*.csv"))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            var output = Path.Combine(outputDir, $"{name}_out.csv");
            
            Scripts_12_08.Run(file, output, limit);
        }
    }
    
    public static async Task RunDirectoryAsync(
        string inputDir,
        string outputDir,
        int? limit = null,
        int parallel = 4,                    // 默认 4~8 核合理，1 太保守了
        CancellationToken ct = default)
    {
        // 1. 确保输出目录存在
        Directory.CreateDirectory(outputDir);
    
        await RunMultiProcess(inputDir, outputDir, limit, parallel);
        // // 2. 获取所有 CSV 文件（支持递归可选）
        // var csvFiles = Directory.EnumerateFiles(inputDir, "*.csv", SearchOption.TopDirectoryOnly);
        // // 如果想递归所有子文件夹：SearchOption.AllDirectories
        //
        // // 3. 并行处理（真正加速的核心）
        // await Parallel.ForEachAsync(csvFiles, new ParallelOptions
        // {
        //     MaxDegreeOfParallelism = Math.Max(1, parallel),
        //     CancellationToken = ct
        // }, async (file, token) =>
        // {
        //     var name = Path.GetFileNameWithoutExtension(file);
        //     var output = Path.Combine(outputDir, $"{name}_out.csv");
        //
        //     // 如果你的 Scripts_12_08.Run 是同步阻塞的（常见情况）
        //     // 用 Task.Run 卸载到线程池
        //     await Task.Run(() => Scripts_12_08.Run(file, output, limit), token);
        //
        //     // 如果你已经把 Run 改成 async（强烈推荐），直接写：
        //     // await Scripts_12_08.RunAsync(file, output, limit, token);
        // });

        Console.WriteLine($"全部处理完成！输出目录：{outputDir}");
    }
    
    
    public static async Task RunMultiProcess(string inputDir, string outputDir, int? limit, int processCount)
    {
        var files = Directory.EnumerateFiles(inputDir, "*.csv", SearchOption.TopDirectoryOnly).ToList();

        processCount = Math.Min(processCount, Environment.ProcessorCount);

        // 按进程数量均分文件
        var groups = files
            .Select((file, index) => new { file, index })
            .GroupBy(x => x.index % processCount)
            .Select(g => g.Select(x => x.file).ToList())
            .ToList();

        var tasks = new List<Task>();
        string exe = Environment.ProcessPath!;
        Console.WriteLine($"exe 路径：{exe}");
        ProcessLogger.Init(Path.Combine(outputDir, $"log.txt"));
        foreach (var group in groups)
        {
            tasks.Add(Task.Run(() =>
            {
                foreach (var file in group)
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    var output = Path.Combine(outputDir, $"{name}.csv");

                    var arg = $"--input \"{file}\" --output \"{output}\" ";
                    if (limit != null)
                    {
                        arg = $"--input \"{file}\" --output \"{output}\" --limit {limit}";
                    }
                    // string dotnetPath = "dotnet";dotnet
                    var psi = new ProcessStartInfo
                    {
                        FileName = exe,
                        Arguments = arg,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    var p = new Process();
                    p.StartInfo = psi;
                    // ✔ 必须在 Start 前绑定事件
                    p.OutputDataReceived += (_, e) =>
                    {
                        if (e.Data != null)
                        {
                            var msg = e.Data;
                            if (msg.IndexOf("fill board", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                var realMsg = $"[子进程] {msg}";
                                Console.WriteLine(realMsg);
                            }
                            ProcessLogger.Log(msg);
                        }
                    };

                    p.ErrorDataReceived += (_, e) =>
                    {
                        if (e.Data != null)
                        {
                            var msg = e.Data;
                            // Console.WriteLine($"[子进程 ERR] {msg}");
                            ProcessLogger.Log(msg);
                        }
                    };
                    p.Start();
                    // ✔ 必须调用 BeginRead，否则不会触发事件
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                    
                    Console.WriteLine("启动子进程");
                    p.WaitForExit();
                }
            }));
        }

        await Task.WhenAll(tasks);
    }
}
