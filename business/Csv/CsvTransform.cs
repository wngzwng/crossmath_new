using business.utils;

namespace business.Csv;

public static class CsvTransform
{
    /// <summary>
    /// 读取 CSV（TIn）→ 转换（selector）→ 写 CSV（TOut）
    /// </summary>
    public static void Transform<TIn, TOut>(
        string inputFile,
        string outputFile,
        Func<TIn, TOut> selector)
        where TIn : class
        where TOut : class
    {
        var inputPath = CsvConfig.ResolvePath(inputFile);
        var outputPath = CsvConfig.ResolvePath(outputFile);

        // 1. 读取
        var records = CsvUtils.Read<TIn>(inputPath);

        // 2. 转换
        var transformed = records.Select(selector).ToList();

        // 3. 写入
        CsvUtils.Write(outputPath, transformed);

        Console.WriteLine($"[CsvTransform] {inputPath} → {outputPath} ({transformed.Count} rows)");
    }


    /// <summary>
    /// 带进度条的 Transform。
    /// </summary>
    public static void TransformWithProgress<TIn, TOut>(
        string inputFile,
        string outputFile,
        Func<TIn, TOut> selector,
        string? description = null)
        where TIn : class
        where TOut : class
    {
        var inputPath = CsvConfig.ResolvePath(inputFile);
        var outputPath = CsvConfig.ResolvePath(outputFile);

        var tqdm = new TqdmProgressPrinter(description ?? "CSV Transform");

        var records = CsvUtils.Read<TIn>(inputPath).ToList();
        int total = records.Count;
        int cur = 0;

        var outputs = new List<TOut>(total);

        foreach (var item in records)
        {
            outputs.Add(selector(item));
            tqdm.Report(++cur, total);
        }

        CsvUtils.Write(outputPath, outputs);

        Console.WriteLine($"[CsvTransform] Completed → {outputPath}");
    }
}
