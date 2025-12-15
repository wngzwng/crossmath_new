using business.Converters;
using business.Csv;
using business.Records;
using business.utils;
using CrossMath.Core.Models;

namespace business.works;

public static class LayoutGenerationPipeline
{
    public static List<LayoutRecord> RunAndExport(
        LayoutGenerationJob job,
        string fileName,
        string? description = null)
    {
        // --- 1. 进度条 ---------------------------------------------------
        var tqdm = new TqdmProgressPrinter(description ?? "Generating Layouts");
        job.ProgressCallback = (cur, total) => tqdm.Report(cur, total);

        // --- 2. 执行 Runner ------------------------------------------------
        var runner = new LayoutGenerationJobRunner();
        var records = new List<LayoutRecord>();

        int id = 1;
        foreach (var layout in runner.Run(job))
        {
            var record = LayoutRecordFactory.FromBoardLayout(id++, layout);
            records.Add(record);
        }

        // --- 3. 写入 CSV ---------------------------------------------------
        var path = CsvConfig.ResolvePath(fileName);
        CsvUtils.Write(path, records);

        Console.WriteLine($"Layouts saved to: {path}");
        return records;
    }

    /// <summary>
    /// 更简单的调用：直接返回结果，不写 CSV
    /// </summary>
    public static List<LayoutRecord> Run(LayoutGenerationJob job,  string? description = null)
    {
        var tqdm = new TqdmProgressPrinter(description ?? "Generating Layouts");
        job.ProgressCallback = (cur, total) => tqdm.Report(cur, total);

        var runner = new LayoutGenerationJobRunner();
        var records = new List<LayoutRecord>();
        int id = 1;

        foreach (var layout in runner.Run(job))
        {
            Console.WriteLine($"\n {job.counter?.ProgressString}");
            layout.LogicPrettyPrint();
            records.Add(LayoutRecordFactory.FromBoardLayout(id++, layout));
        }

        return records;
    }
}