namespace business.utils;

public static class FileSplitter
{
    /// <summary>
    /// 按行拆分文本文件，是 CSV/log 等最常用方式
    /// </summary>
    public static void SplitByLines(
        string inputFile,
        string outputDir,
        int linesPerFile)
    {
        Directory.CreateDirectory(outputDir);

        using var sr = new StreamReader(inputFile);

        int fileIndex = 1;
        int lineCount = 0;
        StreamWriter? sw = null;

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            if (lineCount % linesPerFile == 0)
            {
                sw?.Dispose();

                string outFile = Path.Combine(
                    outputDir,
                    $"{Path.GetFileNameWithoutExtension(inputFile)}_{fileIndex}{Path.GetExtension(inputFile)}");

                sw = new StreamWriter(outFile);
                fileIndex++;
            }

            sw!.WriteLine(line);
            lineCount++;
        }

        sw?.Dispose();
    }

    /// <summary>
    /// 按大小拆分文件（支持任意类型：csv/bin/zip）
    /// </summary>
    public static void SplitBySize(
        string inputFile,
        string outputDir,
        long maxBytesPerFile)
    {
        Directory.CreateDirectory(outputDir);

        using FileStream input = File.OpenRead(inputFile);

        byte[] buffer = new byte[1024 * 1024]; // 1MB
        int read;
        int fileIndex = 1;
        long currentSize = 0;

        FileStream? outStream = null;

        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            if (outStream == null || currentSize >= maxBytesPerFile)
            {
                outStream?.Dispose();

                string outFile = Path.Combine(
                    outputDir,
                    $"{Path.GetFileNameWithoutExtension(inputFile)}_{fileIndex}{Path.GetExtension(inputFile)}");

                outStream = File.Create(outFile);
                currentSize = 0;
                fileIndex++;
            }

            outStream.Write(buffer, 0, read);
            currentSize += read;
        }

        outStream?.Dispose();
    }
}