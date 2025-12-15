namespace business.utils;

public static class FileMerger
{
    /// <summary>
    /// 文本文件合并
    /// </summary>
    public static void MergeTextFiles(IEnumerable<string> inputFiles, string outputFile)
    {
        using var sw = new StreamWriter(outputFile);

        foreach (var file in inputFiles)
        {
            using var sr = new StreamReader(file);
            sw.Write(sr.ReadToEnd());
        }
    }

    /// <summary>
    /// CSV 合并（去除重复 header）
    /// </summary>
    public static void MergeCsvFiles(IEnumerable<string> inputFiles, string outputFile)
    {
        using var sw = new StreamWriter(outputFile);
        bool isFirst = true;

        foreach (var file in inputFiles)
        {
            using var sr = new StreamReader(file);

            string? header = sr.ReadLine();
            if (header == null) continue;

            if (isFirst)
            {
                sw.WriteLine(header);
                isFirst = false;
            }

            string? line;
            while ((line = sr.ReadLine()) != null)
                sw.WriteLine(line);
        }
    }

    /// <summary>
    /// 二进制文件合并
    /// </summary>
    public static void MergeBinaryFiles(IEnumerable<string> inputFiles, string outputFile)
    {
        using var output = File.Create(outputFile);

        foreach (var file in inputFiles)
        {
            using var input = File.OpenRead(file);
            input.CopyTo(output);
        }
    }
}