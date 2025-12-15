namespace business;

public static class FinalBoardCmd
{
    public static async Task Run(
        string? input,
        string? inputDir,
        string? output,
        string? outputDir,
        int? limit,
        int? parallel)
    {
        Console.WriteLine("== Final Board Processor ==");

        if (input != null) // 单文件模式
        {
            if (output == null)
                throw new Exception("--output 是必须的");

            FinalBoardJob.Run(input, output, limit);
            return;
        }

        if (inputDir != null) // 目录模式
        {
            if (outputDir == null)
                throw new Exception("--output-dir 是必须的");
            if (parallel == null)
            {
                FinalBoardJob.RunDirectory(inputDir, outputDir, limit);
            }
            else
            {
                await FinalBoardJob.RunDirectoryAsync(inputDir, outputDir, limit, parallel.Value);
            }
            return;
        }

        throw new Exception("必须指定 --input 或 --input-dir");
    }
}
